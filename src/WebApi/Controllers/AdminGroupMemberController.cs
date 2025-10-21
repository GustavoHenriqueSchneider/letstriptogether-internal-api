using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs.Requests.GroupMember;
using WebApi.DTOs.Responses;
using WebApi.DTOs.Responses.GroupMember;
using WebApi.Models;
using WebApi.Persistence.Interfaces;
using WebApi.Repositories.Interfaces;
using WebApi.Security;

namespace WebApi.Controllers;

// TODO: aplicar CQRS com usecases, mediator com mediatr e clean arc
// TODO: colocar tag de versionamento e descricoes para swagger
// TODO: converter returns de erro em exception

[Authorize(Policy = Policies.Admin)]
[ApiController]
[Route("api/v1/admin/group-members")]
public class AdminGroupMemberController(
    IUnitOfWork unitOfWork,
    IGroupMemberRepository groupMemberRepository,
    IGroupRepository groupRepository,
    IUserRepository userRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10)
    {
        var (groupMembers, hits) = await groupMemberRepository.GetAllAsync(pageNumber, pageSize);

        return Ok(new GetAllGroupMembersResponse
        {
            Data = MapToResponseData(groupMembers),
            Hits = hits
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var groupMember = await groupMemberRepository.GetByIdWithRelationsAsync(id);

        if (groupMember is null)
        {
            return NotFound(new ErrorResponse("Group member not found."));
        }

        return Ok(new GetGroupMemberByIdResponse
        {
            Id = groupMember.Id,
            GroupId = groupMember.GroupId,
            GroupName = groupMember.Group.Name,
            UserId = groupMember.UserId,
            UserName = groupMember.User.Name,
            UserEmail = groupMember.User.Email,
            IsOwner = groupMember.IsOwner,
            CreatedAt = groupMember.CreatedAt,
            UpdatedAt = groupMember.UpdatedAt
        });
    }

    [HttpGet("group/{groupId:guid}")]
    public async Task<IActionResult> GetByGroupId([FromRoute] Guid groupId,
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10)
    {
        var (groupMembers, hits) = await groupMemberRepository.GetByGroupIdAsync(groupId, pageNumber, pageSize);

        return Ok(new GetAllGroupMembersResponse
        {
            Data = MapToResponseData(groupMembers),
            Hits = hits
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGroupMemberRequest request)
    {
        var groupExists = await groupRepository.ExistsByIdAsync(request.GroupId);
        if (!groupExists)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var userExists = await userRepository.ExistsByIdAsync(request.UserId);
        if (!userExists)
        {
            return NotFound(new ErrorResponse("User not found."));
        }
        var isAlreadyMember = await groupMemberRepository.ExistsByGroupAndUserAsync(request.GroupId, request.UserId);
        if (isAlreadyMember)
        {
            return Conflict(new ErrorResponse("User is already a member of this group."));
        }

        var groupMember = new GroupMember(request.GroupId, request.UserId, false);

        await groupMemberRepository.AddAsync(groupMember);
        await unitOfWork.SaveAsync();

        return CreatedAtAction(nameof(Create), new CreateGroupMemberResponse { Id = groupMember.Id });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteById([FromRoute] Guid id)
    {
        var groupMember = await groupMemberRepository.GetByIdAsync(id);

        if (groupMember is null)
        {
            return NotFound(new ErrorResponse("Group member not found."));
        }

        groupMemberRepository.Remove(groupMember);
        await unitOfWork.SaveAsync();

        return NoContent();
    }

    private static IEnumerable<GetAllGroupMembersResponseData> MapToResponseData(IEnumerable<GroupMember> groupMembers)
    {
        return groupMembers.Select(x => new GetAllGroupMembersResponseData
        {
            Id = x.Id,
            GroupId = x.GroupId,
            UserId = x.UserId,
            IsOwner = x.IsOwner,
            CreatedAt = x.CreatedAt
        });
    }
}
