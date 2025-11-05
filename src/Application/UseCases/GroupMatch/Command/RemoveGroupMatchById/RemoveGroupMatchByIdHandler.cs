using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Common;

using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupMatch.Command.RemoveGroupMatchById;

public class RemoveGroupMatchByIdHandler : IRequestHandler<RemoveGroupMatchByIdCommand>
{
    private readonly IGroupMemberDestinationVoteRepository _groupMemberDestinationVoteRepository;
    private readonly IGroupMatchRepository _groupMatchRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public RemoveGroupMatchByIdHandler(
        IGroupMemberDestinationVoteRepository groupMemberDestinationVoteRepository,
        IGroupMatchRepository groupMatchRepository,
        IGroupRepository groupRepository,
        IUnitOfWork unitOfWork,
        IUserRepository userRepository)
    {
        _groupMemberDestinationVoteRepository = groupMemberDestinationVoteRepository;
        _groupMatchRepository = groupMatchRepository;
        _groupRepository = groupRepository;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public async Task Handle(RemoveGroupMatchByIdCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = request.UserId;
        if (!await _userRepository.ExistsByIdAsync(currentUserId, cancellationToken))
        {
            throw new NotFoundException("User not found.");
        }

        var group = await _groupRepository.GetGroupWithMembersAndMatchesAsync(request.GroupId, cancellationToken);
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
        
        var vote = await _groupMemberDestinationVoteRepository.GetByMemberAndDestinationAsync(
            groupMember.Id, matchToRemove.DestinationId, cancellationToken);
        
        if (vote is null)
        {
            throw new NotFoundException("The vote was not found.");
        }
        
        vote.SetApproved(false);
        
        _groupMatchRepository.Remove(matchToRemove);
        _groupMemberDestinationVoteRepository.Update(vote);
        
        await _unitOfWork.SaveAsync(cancellationToken);
    }
}
