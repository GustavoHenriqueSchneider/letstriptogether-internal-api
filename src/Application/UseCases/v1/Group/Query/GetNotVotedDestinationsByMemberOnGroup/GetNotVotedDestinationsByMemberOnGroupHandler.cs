using Application.Common.Exceptions;
using Domain.Aggregates.DestinationAggregate;
using Domain.Aggregates.GroupAggregate;
using Domain.Aggregates.UserAggregate;
using MediatR;

namespace Application.UseCases.Group.Query.GetNotVotedDestinationsByMemberOnGroup;

public class GetNotVotedDestinationsByMemberOnGroupHandler(
    IDestinationRepository destinationRepository,
    IGroupRepository groupRepository,
    IUserRepository userRepository)
    : IRequestHandler<GetNotVotedDestinationsByMemberOnGroupQuery, GetNotVotedDestinationsByMemberOnGroupResponse>
{
    public async Task<GetNotVotedDestinationsByMemberOnGroupResponse> Handle(GetNotVotedDestinationsByMemberOnGroupQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = request.UserId;
        if (!await userRepository.ExistsByIdAsync(currentUserId, cancellationToken))
        {
            throw new NotFoundException("User not found.");
        }
        
        var group = await groupRepository.GetGroupWithMembersPreferencesAsync(request.GroupId, cancellationToken);
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
        
        var (destinations, hits) = await destinationRepository.GetNotVotedByUserInGroupAsync(
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
