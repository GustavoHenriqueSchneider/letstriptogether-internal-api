using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs.Responses;
using WebApi.DTOs.Responses.Admin.Group;
using WebApi.DTOs.Responses.Admin.User;
using WebApi.DTOs.Responses.User;
using WebApi.Models.Aggregates;
using WebApi.Models.ValueObjects;
using WebApi.Repositories.Interfaces;
using WebApi.Security;

namespace WebApi.Controllers.Admin;

// TODO: aplicar CQRS com usecases, mediator com mediatr e clean arc
// TODO: colocar tag de versionamento e descricoes para swagger
// TODO: converter returns de erro em exception

[ApiController]
[Authorize(Policy = Policies.Admin)]
[Route("api/v1/admin/groups")]
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

