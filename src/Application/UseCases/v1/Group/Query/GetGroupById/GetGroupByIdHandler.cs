using Application.Common.Exceptions;
using Domain.Aggregates.GroupAggregate;
using MediatR;

namespace Application.UseCases.v1.Group.Query.GetGroupById;

public class GetGroupByIdHandler(
    IGroupPreferenceRepository groupPreferenceRepository,
    IGroupRepository groupRepository)
    : IRequestHandler<GetGroupByIdQuery, GetGroupByIdResponse>
{
    public async Task<GetGroupByIdResponse> Handle(GetGroupByIdQuery request, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetGroupWithMembersAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            throw new NotFoundException("Group not found.");
        }

        var member = group.Members.SingleOrDefault(x => x.UserId == request.UserId);
        if (member is null)
        {
            throw new BadRequestException("You are not a member of this group.");
        }

        var groupPreferences = await groupPreferenceRepository.GetByGroupIdAsync(request.GroupId, cancellationToken)
            ?? throw new BadRequestException("Invalid preferences");

        return new GetGroupByIdResponse
        {
            Name = group.Name,
            TripExpectedDate = group.TripExpectedDate,
            IsCurrentMemberOwner = member.IsOwner,
            Preferences = new GetGroupByIdPreferenceResponse
            {
                LikesShopping = groupPreferences.LikesShopping,
                LikesGastronomy = groupPreferences.LikesGastronomy,
                Culture = groupPreferences.Culture,
                Entertainment = groupPreferences.Entertainment,
                PlaceTypes = groupPreferences.PlaceTypes,
            },
            CreatedAt = group.CreatedAt,
            UpdatedAt = group.UpdatedAt
        };
    }
}
