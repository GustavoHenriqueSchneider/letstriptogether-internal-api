using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Context.Interfaces;
using WebApi.DTOs.Requests.Group;
using WebApi.DTOs.Responses;
using WebApi.DTOs.Responses.Group;
using WebApi.Models;
using WebApi.Persistence.Interfaces;
using WebApi.Repositories.Interfaces;
using WebApi.Security;

namespace WebApi.Controllers;

// TODO: aplicar CQRS com usecases, mediator com mediatr e clean arc
// TODO: colocar tag de versionamento e descricoes para swagger
// TODO: converter returns de erro em exception

[Authorize]
[ApiController]
[Route("api/v1/groups")]
public class GroupController(
    IGroupRepository groupRepository,
    IGroupMemberRepository groupMemberRepository,
    IApplicationUserContext currentUser,
    IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGroupRequest request)
    {
        var currentUserId = currentUser.GetId();
        
        var group = new Group(request.Name, request.TripExpectedDate);
        
        await groupRepository.AddAsync(group);
        await unitOfWork.SaveAsync();
        
        // Make the creator the owner
        var groupMember = new GroupMember(group.Id, currentUserId, isOwner: true);
        await groupMemberRepository.AddAsync(groupMember);
        await unitOfWork.SaveAsync();
        
        return CreatedAtAction(nameof(Create), new CreateGroupResponse(group.Id));
    }

    [HttpGet]
    [Authorize(Policy = Policies.Admin)]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10)
    {
        var (groups, hits) = await groupRepository.GetAllAsync(pageNumber, pageSize);

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

    [HttpGet("{id:guid}")]
    [Authorize(Policy = Policies.Admin)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var group = await groupRepository.GetByIdAsync(id);

        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        return Ok(new GetGroupByIdResponse
        {
            Name = group.Name,
            TripExpectedDate = group.TripExpectedDate,
            CreatedAt = group.CreatedAt,
            UpdatedAt = group.UpdatedAt
        });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateGroupRequest request)
    {
        var group = await GetGroupWithOwnerValidation(id);
        if (group is null) return NotFound(new ErrorResponse("Group not found."));

        group.Update(request.Name, request.TripExpectedDate);
        groupRepository.Update(group);
        await unitOfWork.SaveAsync();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var group = await GetGroupWithOwnerValidation(id);
        if (group is null) return NotFound(new ErrorResponse("Group not found."));

        groupRepository.Remove(group);
        await unitOfWork.SaveAsync();

        return NoContent();
    }

    private async Task<Group?> GetGroupWithOwnerValidation(Guid groupId)
    {
        var currentUserId = currentUser.GetId();
        var group = await groupRepository.GetGroupWithMembersAsync(groupId);
        
        if (group is null) return null;

        var currentUserMember = group.Members.SingleOrDefault(m => m.UserId == currentUserId);
        if (currentUserMember is null || !currentUserMember.IsOwner) return null;

        return group;
    }
}

