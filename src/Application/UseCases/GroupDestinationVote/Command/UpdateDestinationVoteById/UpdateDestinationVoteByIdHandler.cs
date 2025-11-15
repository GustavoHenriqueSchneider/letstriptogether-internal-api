using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Domain.Aggregates.GroupAggregate;
using Domain.Aggregates.UserAggregate;
using Domain.Common;
using Domain.Events;
using MediatR;

namespace Application.UseCases.GroupDestinationVote.Command.UpdateDestinationVoteById;

public class UpdateDestinationVoteByIdHandler(
    IGroupMatchRepository groupMatchRepository,
    IGroupMemberDestinationVoteRepository groupMemberDestinationVoteRepository,
    IGroupRepository groupRepository,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository,
    INotificationService notificationService)
    : IRequestHandler<UpdateDestinationVoteByIdCommand>
{
    public async Task Handle(UpdateDestinationVoteByIdCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = request.UserId;
        var user = await userRepository.GetUserWithGroupMembershipsAsync(currentUserId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        var groupMember = user.GroupMemberships.SingleOrDefault(m => m.GroupId == request.GroupId);
        if (groupMember is null)
        {
            throw new BadRequestException("You are not a member of this group.");
        }

        var vote = await groupMemberDestinationVoteRepository.GetByIdAsync(request.DestinationVoteId, cancellationToken);

        if (vote is null)
        {
            throw new NotFoundException("Vote not found.");
        }

        if (vote.GroupMemberId != groupMember.Id)
        {
            throw new BadRequestException("You are not a owner of this vote.");
        }
        
        var match = await groupMatchRepository.GetByGroupAndDestinationAsync(request.GroupId, vote.DestinationId, cancellationToken);
        if (match is not null)
        {
            throw new BadRequestException("There is already a match with this vote, you can not change it.");
        }

        vote.SetApproved(request.IsApproved);
        
        groupMemberDestinationVoteRepository.Update(vote);
        await unitOfWork.SaveAsync(cancellationToken);

        if (!vote.IsApproved)
        {
            return;
        }
        
        var group = await groupRepository.GetGroupWithMembersVotesAndMatchesAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            throw new NotFoundException("Group not found.");
        }
            
        try
        {
            match = group.CreateMatch(vote.DestinationId);
            await groupMatchRepository.AddAsync(match, cancellationToken);
            await unitOfWork.SaveAsync(cancellationToken);
            
            var notificationData = new { groupId = group.Id, destinationId = vote.DestinationId };
                
            var notificationTasks = group.Members.Select(member =>
                notificationService.SendNotificationAsync(member.UserId, 
                    NotificationEvents.GroupMatchCreated, notificationData, cancellationToken));
                
            await Task.WhenAll(notificationTasks).ConfigureAwait(false);
        }
        catch
        {
            // purposed ignored
        }
    }
}
