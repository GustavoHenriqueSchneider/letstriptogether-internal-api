using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;
using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroup.Query.AdminGetGroupById;

public class AdminGetGroupByIdHandler(IGroupRepository groupRepository)
    : IRequestHandler<AdminGetGroupByIdQuery, AdminGetGroupByIdResponse>
{
    public async Task<AdminGetGroupByIdResponse> Handle(AdminGetGroupByIdQuery request, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetGroupWithPreferencesAsync(request.GroupId, cancellationToken);

        if (group is null)
        {
            throw new NotFoundException("Group not found.");
        }

        var groupPreferences = new GroupPreference(group.Preferences);

        return new AdminGetGroupByIdResponse
        {
            Name = group.Name,
            TripExpectedDate = group.TripExpectedDate,
            Preferences = new AdminGetGroupByIdPreferenceResponse
            {
                LikesCommercial = groupPreferences.LikesCommercial,
                Food = groupPreferences.Food,
                Culture = groupPreferences.Culture,
                Entertainment = groupPreferences.Entertainment,
                PlaceTypes = groupPreferences.PlaceTypes,
            },
            CreatedAt = group.CreatedAt,
            UpdatedAt = group.UpdatedAt
        };
    }
}
