using Application.Common.Exceptions;
using Domain.Aggregates.GroupAggregate;
using Domain.Aggregates.UserAggregate;
using Domain.Common;
using MediatR;

namespace Application.UseCases.v1.GroupMatch.Command.RemoveGroupMatchById;

public class RemoveGroupMatchByIdHandler(
    IGroupMemberDestinationVoteRepository groupMemberDestinationVoteRepository,
    IGroupMatchRepository groupMatchRepository,
    IGroupRepository groupRepository,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository)
    : IRequestHandler<RemoveGroupMatchByIdCommand>
{
    public async Task Handle(RemoveGroupMatchByIdCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = request.UserId;
        if (!await userRepository.ExistsByIdAsync(currentUserId, cancellationToken))
        {
            throw new NotFoundException("User not found.");
        }

        var group = await groupRepository.GetGroupWithMembersAndMatchesAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            throw new NotFoundException("Group not found.");
        }

        var groupMember = group.Members.SingleOrDefault(x => x.UserId == currentUserId);
        if (groupMember is null)
        {
            throw new BadRequestException("You are not a member of this group.");
        }

        var matchToRemove = group.Matches.SingleOrDefault(m => m.Id == request.MatchId);
        if (matchToRemove is null)
        {
            throw new NotFoundException("The match was not found for this group.");
        }
        
        var vote = await groupMemberDestinationVoteRepository.GetByMemberAndDestinationAsync(
            groupMember.Id, matchToRemove.DestinationId, cancellationToken);
        
        if (vote is null)
        {
            throw new NotFoundException("The vote was not found.");
        }
        
        vote.SetApproved(false);
        
        groupMatchRepository.Remove(matchToRemove);
        groupMemberDestinationVoteRepository.Update(vote);
        
        await unitOfWork.SaveAsync(cancellationToken);
    }
}
