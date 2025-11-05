using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;

using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;

namespace LetsTripTogether.InternalApi.Application.UseCases.AdminGroup.Query.AdminGetGroupById;

public class AdminGetGroupByIdHandler : IRequestHandler<AdminGetGroupByIdQuery, AdminGetGroupByIdResponse>
{
    private readonly IGroupRepository _groupRepository;

    public AdminGetGroupByIdHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task<AdminGetGroupByIdResponse> Handle(AdminGetGroupByIdQuery request, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetGroupWithPreferencesAsync(request.GroupId, cancellationToken);

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
