using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;

using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;

namespace LetsTripTogether.InternalApi.Application.UseCases.Group.Query.GetGroupById;

public class GetGroupByIdHandler : IRequestHandler<GetGroupByIdQuery, GetGroupByIdResponse>
{
    private readonly IGroupPreferenceRepository _groupPreferenceRepository;
    private readonly IGroupRepository _groupRepository;

    public GetGroupByIdHandler(
        IGroupPreferenceRepository groupPreferenceRepository,
        IGroupRepository groupRepository)
    {
        _groupPreferenceRepository = groupPreferenceRepository;
        _groupRepository = groupRepository;
    }

    public async Task<GetGroupByIdResponse> Handle(GetGroupByIdQuery request, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetGroupWithMembersAsync(request.GroupId, cancellationToken);

        if (group is null)
        {
            throw new NotFoundException("Group not found.");
        }

        var isMember = group.Members.Any(x => x.UserId == request.UserId);

        if (!isMember)
        {
            throw new BadRequestException("You are not a member of this group.");
        }

        var groupPreferences = await _groupPreferenceRepository.GetByGroupIdAsync(request.GroupId, cancellationToken)
            ?? throw new InvalidOperationException("Invalid preferences");

        return new GetGroupByIdResponse
        {
            Name = group.Name,
            TripExpectedDate = group.TripExpectedDate,
            Preferences = new GetGroupByIdPreferenceResponse
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
