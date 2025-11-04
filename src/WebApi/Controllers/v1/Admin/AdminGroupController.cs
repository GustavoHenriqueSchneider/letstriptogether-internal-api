using LetsTripTogether.InternalApi.Application.Common.Policies;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;
using LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses;
using LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses.Admin.Group;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetsTripTogether.InternalApi.WebApi.Controllers.v1.Admin;

// TODO: aplicar CQRS com usecases, mediator com mediatr e clean arc
// TODO: colocar tag de versionamento e descricoes para swagger
// TODO: converter returns de erro em exception

[ApiController]
[Authorize(Policy = Policies.Admin)]
[Route("api/v{version:apiVersion}/admin/groups")]
public class AdminGroupController(
    IGroupRepository groupRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> AdminGetAllGroups([FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10)
    {
        var (groups, hits) = await groupRepository.GetAllAsync(pageNumber, pageSize);

        return Ok(new AdminGetAllGroupsResponse
        {
            Data = groups.Select(x => new AdminGetAllGroupsResponseData
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt
            }),
            Hits = hits
        });
    }

    [HttpGet("{groupId:guid}")]
    public async Task<IActionResult> AdminGetGroupById([FromRoute] Guid groupId)
    {
        var group = await groupRepository.GetGroupWithPreferencesAsync(groupId);

        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        try
        {
            var groupPreferences = new GroupPreference(group.Preferences);

            return Ok(new AdminGetGroupByIdResponse
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
            });
        }
        catch (InvalidOperationException ex) when (ex.Message.StartsWith("Invalid"))
        {
            return UnprocessableEntity(
                new ErrorResponse("Invalid preferences are filled for this group, please fix it."));
        }
    }
}

