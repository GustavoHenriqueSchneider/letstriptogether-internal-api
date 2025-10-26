using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Context.Interfaces;
using WebApi.DTOs.Responses;
using WebApi.DTOs.Responses.Group;
using WebApi.Repositories.Interfaces;

namespace WebApi.Controllers;

// TODO: aplicar CQRS com usecases, mediator com mediatr e clean arc
// TODO: colocar tag de versionamento e descricoes para swagger
// TODO: converter returns de erro em exception

[ApiController]
[Authorize]
[Route("api/v1/groups")]
public class GroupController(
    IApplicationUserContext currentUser,
    IGroupRepository groupRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllGroups([FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10)
    {
        var (groups, hits) = await groupRepository.GetAllGroupsByUserIdAsync(
            currentUser.GetId(), pageNumber, pageSize);

        return Ok(new GetAllGroupsResponse
        {
            Data = groups.Select(x => new GetAllGroupsResponseData
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt
            }),
            Hits = hits
        });
    }

    [HttpGet("{groupId:guid}")]
    public async Task<IActionResult> GetGroupById([FromRoute] Guid groupId)
    {
        var group = await groupRepository.GetGroupWithMembersAsync(groupId);

        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var isMember = group.Members.Any(x => x.UserId == currentUser.GetId());

        if (!isMember)
        {
            return BadRequest(new ErrorResponse("You are not a member of this group."));
        }

        return Ok(new GetGroupByIdResponse
        {
            Name = group.Name,
            TripExpectedDate = group.TripExpectedDate,
            CreatedAt = group.CreatedAt,
            UpdatedAt = group.UpdatedAt
        });
    }
}

