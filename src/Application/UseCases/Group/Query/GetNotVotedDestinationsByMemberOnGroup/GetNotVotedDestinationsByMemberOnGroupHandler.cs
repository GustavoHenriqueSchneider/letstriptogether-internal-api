using LetsTripTogether.InternalApi.Domain.Aggregates.DestinationAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;

using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;

namespace LetsTripTogether.InternalApi.Application.UseCases.Group.Query.GetNotVotedDestinationsByMemberOnGroup;

public class GetNotVotedDestinationsByMemberOnGroupHandler : IRequestHandler<GetNotVotedDestinationsByMemberOnGroupQuery, GetNotVotedDestinationsByMemberOnGroupResponse>
{
    private readonly IDestinationRepository _destinationRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;

    public GetNotVotedDestinationsByMemberOnGroupHandler(
        IDestinationRepository destinationRepository,
        IGroupRepository groupRepository,
        IUserRepository userRepository)
    {
        _destinationRepository = destinationRepository;
        _groupRepository = groupRepository;
        _userRepository = userRepository;
    }

    public async Task<GetNotVotedDestinationsByMemberOnGroupResponse> Handle(GetNotVotedDestinationsByMemberOnGroupQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = request.UserId;
        if (!await _userRepository.ExistsByIdAsync(currentUserId, cancellationToken))
        {
            throw new NotFoundException("User not found.");
        }
        
        var group = await _groupRepository.GetGroupWithMembersPreferencesAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            throw new NotFoundException("Group not found.");
        }
        
        var isGroupMember = group.Members.Any(m => m.UserId == currentUserId);
        if (!isGroupMember)
        {
            throw new BadRequestException("You are not a member of this group.");
        }
        
        var groupPreferences = group.Preferences.ToList();
        
        var (destinations, hits) = await _destinationRepository.GetNotVotedByUserInGroupAsync(
            currentUserId, request.GroupId, groupPreferences, request.PageNumber, request.PageSize, cancellationToken);

        return new GetNotVotedDestinationsByMemberOnGroupResponse
        {
            Data = destinations.Select(x => new GetNotVotedDestinationsByMemberOnGroupResponseData
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt
            }),
            Hits = hits
        };
    }
}
