using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WebApi.Context.Interfaces;
using WebApi.DTOs.Requests.Group;
using WebApi.DTOs.Responses;
using WebApi.DTOs.Responses.Group;
using WebApi.Models;
using WebApi.Persistence.Interfaces;
using WebApi.Repositories.Implementations;
using WebApi.Repositories.Interfaces;

namespace WebApi.Controllers;

// TODO: aplicar CQRS com usecases, mediator com mediatr e clean arc
// TODO: colocar tag de versionamento e descricoes para swagger
// TODO: converter returns de erro em exception

[ApiController]
[Authorize]
[Route("api/v1/groups")]
public class GroupController(
    IGroupRepository groupRepository,
    IApplicationUserContext currentUser,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request)
    {
        var currentUserId = currentUser.GetId();
        var existsUser = await userRepository.ExistsByIdAsync(currentUserId);

        if (!existsUser)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var group = new Group(request.Name, request.TripExpectedDate.ToUniversalTime());

        var groupMember = new GroupMember
        {
            GroupId = group.Id,
            UserId = currentUserId,
            IsOwner = true
        };

        group.AddMember(groupMember);
        await groupRepository.AddAsync(group);

        await unitOfWork.SaveAsync();
        return CreatedAtAction(nameof(CreateGroup), new CreateGroupResponse { Id = group.Id });
    }

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

    [HttpPut("{groupId:guid}")]
    public async Task<IActionResult> UpdateGroupById([FromRoute] Guid groupId, 
        [FromBody] UpdateGroupRequest request)
    {
        var currentUserId = currentUser.GetId();

        if (!await userRepository.ExistsByIdAsync(currentUserId))
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var group = await groupRepository.GetGroupWithMembersAsync(groupId);

        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var currentUserMember = group.Members.SingleOrDefault(m => m.UserId == currentUserId);

        if (currentUserMember is null)
        {
            return BadRequest(new ErrorResponse("You are not a member of this group."));
        }

        if (!currentUserMember.IsOwner)
        {
            return BadRequest(new ErrorResponse("Only the group owner can update group data."));
        }

        group.Update(request.Name, request.TripExpectedDate);
        groupRepository.Update(group);
        await unitOfWork.SaveAsync();

        return NoContent();
    }

    [HttpDelete("{groupId:guid}")]
    public async Task<IActionResult> DeleteGroupById([FromRoute] Guid groupId)
    {
        var currentUserId = currentUser.GetId();

        if (!await userRepository.ExistsByIdAsync(currentUserId))
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var group = await groupRepository.GetGroupWithMembersAsync(groupId);

        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var currentUserMember = group.Members.SingleOrDefault(m => m.UserId == currentUserId);

        if (currentUserMember is null)
        {
            return BadRequest(new ErrorResponse("You are not a member of this group."));
        }

        if (!currentUserMember.IsOwner)
        {
            return BadRequest(new ErrorResponse("Only the group owner can delete the group."));
        }

        groupRepository.Remove(group);
        await unitOfWork.SaveAsync();

        return NoContent();
    }
}

