using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Domain.Aggregates.DestinationAggregate;
using Domain.Aggregates.GroupAggregate;
using Domain.Aggregates.GroupAggregate.Entities;
using Domain.Aggregates.UserAggregate;
using Domain.Common;
using Domain.Security;
using MediatR;

namespace Application.UseCases.GroupDestinationVote.Command.VoteAtDestinationForGroupId;

public class VoteAtDestinationForGroupIdHandler(
    IDestinationRepository destinationRepository,
    IGroupMatchRepository groupMatchRepository,
    IGroupMemberDestinationVoteRepository groupMemberDestinationVoteRepository,
    IGroupRepository groupRepository,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository,
    INotificationService notificationService)
    : IRequestHandler<VoteAtDestinationForGroupIdCommand, VoteAtDestinationForGroupIdResponse>
{
    public async Task<VoteAtDestinationForGroupIdResponse> Handle(VoteAtDestinationForGroupIdCommand request, CancellationToken cancellationToken)
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

        var destinationExists = await destinationRepository.ExistsByIdAsync(request.DestinationId, cancellationToken);

        if (!destinationExists)
        {
            throw new NotFoundException("Destination not found.");
        }

        var existsVote = 
            await groupMemberDestinationVoteRepository.ExistsByGroupMemberDestinationVoteByIdsAsync(
                groupMember.Id, request.DestinationId, cancellationToken);
        
        if (existsVote)
        {
            throw new ConflictException("Vote already exists for the informed group and destination ids.");
        }

        var vote = new GroupMemberDestinationVote(groupMember.Id, request.DestinationId, request.IsApproved);
        await groupMemberDestinationVoteRepository.AddAsync(vote, cancellationToken);
        await unitOfWork.SaveAsync(cancellationToken);

        if (!vote.IsApproved)
        {
            return new VoteAtDestinationForGroupIdResponse { Id = vote.Id };
        }

        var group = await groupRepository.GetGroupWithMembersVotesAndMatchesAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            throw new NotFoundException("Group not found.");
        }
        
        try
        {
            var match = group.CreateMatch(request.DestinationId);
            await groupMatchRepository.AddAsync(match, cancellationToken);
            await unitOfWork.SaveAsync(cancellationToken);
            
            var notificationData = new { groupId = group.Id };
                
            var notificationTasks = group.Members.Select(member =>
                notificationService.SendNotificationAsync(
                    member.UserId,
                    NotificationEvents.GroupMatchCreated,
                    notificationData,
                    cancellationToken));
                
            await Task.WhenAll(notificationTasks).ConfigureAwait(false);
        }
        catch
        {
            // purposed ignored
        }

        return new VoteAtDestinationForGroupIdResponse { Id = vote.Id };
    }
}
