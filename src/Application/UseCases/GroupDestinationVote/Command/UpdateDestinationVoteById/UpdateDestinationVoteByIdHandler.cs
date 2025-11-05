using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Common;

using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupDestinationVote.Command.UpdateDestinationVoteById;

public class UpdateDestinationVoteByIdHandler(
    IGroupMatchRepository groupMatchRepository,
    IGroupMemberDestinationVoteRepository groupMemberDestinationVoteRepository,
    IGroupRepository groupRepository,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository)
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

        var existsGroup = await groupRepository.ExistsByIdAsync(request.GroupId, cancellationToken);
        if (!existsGroup)
        {
            throw new NotFoundException("Group not found.");
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
            
            // TODO: criar service de notifica??o
        }
        catch
        {
            // purposed ignored
        }
    }
}
