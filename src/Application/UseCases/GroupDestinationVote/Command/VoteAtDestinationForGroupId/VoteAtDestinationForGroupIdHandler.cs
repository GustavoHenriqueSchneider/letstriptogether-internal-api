using LetsTripTogether.InternalApi.Domain.Aggregates.DestinationAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Common;

using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupDestinationVote.Command.VoteAtDestinationForGroupId;

public class VoteAtDestinationForGroupIdHandler(
    IDestinationRepository destinationRepository,
    IGroupMatchRepository groupMatchRepository,
    IGroupMemberDestinationVoteRepository groupMemberDestinationVoteRepository,
    IGroupRepository groupRepository,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository)
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
                
            // TODO: criar service de notifica??o
        }
        catch
        {
            // purposed ignored
        }

        return new VoteAtDestinationForGroupIdResponse { Id = vote.Id };
    }
}
