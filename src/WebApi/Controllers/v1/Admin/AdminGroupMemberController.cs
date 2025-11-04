using LetsTripTogether.InternalApi.Application.Common.Policies;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Common;
using LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses;
using LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses.Admin.GroupMember;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetsTripTogether.InternalApi.WebApi.Controllers.v1.Admin;

[ApiController]
[Authorize(Policy = Policies.Admin)]
[Route("api/v{version:apiVersion}/admin/groups/{groupId:guid}/members")]
public class AdminGroupMemberController(
    IUnitOfWork unitOfWork,
    IGroupRepository groupRepository,
    IGroupMemberDestinationVoteRepository groupMemberDestinationVoteRepository,
    IGroupMemberRepository groupMemberRepository,
    IGroupPreferenceRepository groupPreferenceRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> AdminGetAllGroupMembersById([FromRoute] Guid groupId,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var groupExists = await groupRepository.ExistsByIdAsync(groupId, cancellationToken);

        if (!groupExists)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var (groupMembers, hits) =
            await groupMemberRepository.GetAllByGroupIdAsync(groupId, pageNumber, pageSize, cancellationToken);

        return Ok(new AdminGetAllGroupMembersByIdResponse
        {
            Data = groupMembers.Select(x => new AdminGetAllGroupMembersByIdResponseData
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt
            }),
            Hits = hits
        });
    }

    [HttpGet("{memberId:guid}")]
    public async Task<IActionResult> AdminGetGroupMemberById([FromRoute] Guid groupId, 
        [FromRoute] Guid memberId, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetGroupWithMembersAsync(groupId, cancellationToken);

        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var groupMember = group.Members.SingleOrDefault(x => x.Id == memberId);

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

    [HttpDelete("{memberId:guid}")]
    public async Task<IActionResult> AdminRemoveGroupMemberById([FromRoute] Guid groupId,
        [FromRoute] Guid memberId, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetGroupWithMembersPreferencesAsync(groupId, cancellationToken);
        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var userToRemove = group.Members.SingleOrDefault(m => m.Id == memberId);
        if (userToRemove is null)
        {
            return NotFound(new ErrorResponse("The user is not a member of this group."));
        }

        if (userToRemove.IsOwner)
        {
            return BadRequest(new ErrorResponse("It is not possible to remove the owner of group."));
        }

        group.RemoveMember(userToRemove);
        
        groupRepository.Update(group);
        groupMemberRepository.Remove(userToRemove);
        groupPreferenceRepository.Update(group.Preferences);
        
        await unitOfWork.SaveAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("{memberId:guid}/destination-votes")]
    public async Task<IActionResult> AdminGetGroupMemberAllDestinationVotesById([FromRoute] Guid groupId,
        [FromRoute] Guid memberId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var group = await groupRepository.GetGroupWithMembersAsync(groupId, cancellationToken);

        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var isGroupMember = group.Members.Any(m => m.Id == memberId);

        if (!isGroupMember)
        {
            return NotFound(new ErrorResponse("The user is not a member of this group."));
        }

        var (votes, hits) = await groupMemberDestinationVoteRepository.GetByMemberIdAsync(memberId,
            pageNumber, pageSize, cancellationToken);

        return Ok(new AdminGetGroupMemberAllDestinationVotesByIdResponse
        {
            Data = votes.Select(x => new AdminGetGroupMemberAllDestinationVotesByIdResponseData
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt
            }),
            Hits = hits
        });
    }
}