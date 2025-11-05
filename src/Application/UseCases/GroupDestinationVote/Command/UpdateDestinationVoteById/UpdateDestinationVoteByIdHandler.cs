using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Common;

using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupDestinationVote.Command.UpdateDestinationVoteById;

public class UpdateDestinationVoteByIdHandler : IRequestHandler<UpdateDestinationVoteByIdCommand>
{
    private readonly IGroupMatchRepository _groupMatchRepository;
    private readonly IGroupMemberDestinationVoteRepository _groupMemberDestinationVoteRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public UpdateDestinationVoteByIdHandler(
        IGroupMatchRepository groupMatchRepository,
        IGroupMemberDestinationVoteRepository groupMemberDestinationVoteRepository,
        IGroupRepository groupRepository,
        IUnitOfWork unitOfWork,
        IUserRepository userRepository)
    {
        _groupMatchRepository = groupMatchRepository;
        _groupMemberDestinationVoteRepository = groupMemberDestinationVoteRepository;
        _groupRepository = groupRepository;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public async Task Handle(UpdateDestinationVoteByIdCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = request.UserId;
        var user = await _userRepository.GetUserWithGroupMembershipsAsync(currentUserId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        var groupMember = user.GroupMemberships.SingleOrDefault(m => m.GroupId == request.GroupId);
        if (groupMember is null)
        {
            throw new BadRequestException("You are not a member of this group.");
        }

        var existsGroup = await _groupRepository.ExistsByIdAsync(request.GroupId, cancellationToken);
        if (!existsGroup)
        {
            throw new NotFoundException("Group not found.");
        }

        var vote = await _groupMemberDestinationVoteRepository.GetByIdAsync(request.DestinationVoteId, cancellationToken);

        if (vote is null)
        {
            throw new NotFoundException("Vote not found.");
        }

        if (vote.GroupMemberId != groupMember.Id)
        {
            throw new BadRequestException("You are not a owner of this vote.");
        }
        
        var match = await _groupMatchRepository.GetByGroupAndDestinationAsync(request.GroupId, vote.DestinationId, cancellationToken);
        if (match is not null)
        {
            throw new BadRequestException("There is already a match with this vote, you can not change it.");
        }

        vote.SetApproved(request.IsApproved);
        
        _groupMemberDestinationVoteRepository.Update(vote);
        await _unitOfWork.SaveAsync(cancellationToken);

        if (!vote.IsApproved)
        {
            return;
        }
        
        var group = await _groupRepository.GetGroupWithMembersVotesAndMatchesAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            throw new NotFoundException("Group not found.");
        }
            
        try
        {
            match = group.CreateMatch(vote.DestinationId);
            await _groupMatchRepository.AddAsync(match, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            
            // TODO: criar service de notifica??o
        }
        catch
        {
            // purposed ignored
        }
    }
}
