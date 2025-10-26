using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs.Responses;
using WebApi.DTOs.Responses.Admin.GroupMember;
using WebApi.Persistence.Interfaces;
using WebApi.Repositories.Interfaces;
using WebApi.Security;

namespace WebApi.Controllers.Admin;

[ApiController]
[Authorize(Policy = Policies.Admin)]
[Route("api/v1/admin/groups/{groupId:guid}/members")]
public class AdminGroupMemberController(
    IUnitOfWork unitOfWork,
    IGroupRepository groupRepository,
    IGroupMemberRepository groupMemberRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> AdminGetAllGroupMembersById([FromRoute] Guid groupId,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var groupExists = await groupRepository.ExistsByIdAsync(groupId);

        if (!groupExists)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var (groupMembers, hits) =
            await groupMemberRepository.GetAllByGroupIdAsync(groupId, pageNumber, pageSize);

        return Ok(new AdminGetAllGroupMembersResponse
        {
            Data = groupMembers.Select(x => new AdminGetAllGroupMembersResponseData
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt
            }),
            Hits = hits
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> AdminGetGroupMemberById([FromRoute] Guid groupId, [FromRoute] Guid id)
    {
        var group = await groupRepository.GetGroupWithMembersAsync(groupId);

        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var groupMember = group.Members.SingleOrDefault(x => x.Id == id);

        if (groupMember is null)
        {
            return NotFound(new ErrorResponse("Group member not found."));
        }

        return Ok(new AdminGetGroupMemberByIdResponse
        {
            UserId = groupMember.UserId,
            IsOwner = groupMember.IsOwner,
            CreatedAt = groupMember.CreatedAt,
            UpdatedAt = groupMember.UpdatedAt
        });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> AdminRemoveGroupMemberById([FromRoute] Guid groupId,
        [FromRoute] Guid id)
    {
        var group = await groupRepository.GetGroupWithMembersAsync(groupId);

        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var userToRemove = group.Members.SingleOrDefault(m => m.Id == id);

        if (userToRemove is null)
        {
            return NotFound(new ErrorResponse("The user is not a member of this group."));
        }

        groupMemberRepository.Remove(userToRemove);
        await unitOfWork.SaveAsync();

        return NoContent();
    }
}