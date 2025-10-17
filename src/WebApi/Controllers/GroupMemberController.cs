using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Context.Interfaces;
using WebApi.DTOs.Requests;
using WebApi.DTOs.Responses;
using WebApi.Models;
using WebApi.Persistence.Interfaces;
using WebApi.Repositories.Interfaces;

namespace WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/groups")]
public class GroupMemberController(
    IUnitOfWork unitOfWork,
    IApplicationUserContext currentUser,
    IUserRepository userRepository,
    IGroupRepository groupRepository,
    IGroupMemberRepository groupMemberRepository) : ControllerBase
{
    [HttpDelete("{groupId:guid}/members")]
    public async Task<IActionResult> RemoveUserFromGroup(
        [FromRoute] Guid groupId, 
        [FromBody] RemoveUserFromGroupRequest request)
    {
        // Validar se usuario informado no accesstoken existe
        var currentUserId = currentUser.GetId();
        var currentUserEntity = await userRepository.GetByIdAsync(currentUserId);
        if (currentUserEntity is null)
        {
            return NotFound(new ErrorResponse("Current user not found."));
        }

        // Validar se grupo informado na request existe
        var group = await groupRepository.GetGroupWithMembersAsync(groupId);
        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        // Validar se usuario informado no accesstoken é membro do grupo
        var currentUserMember = group.Members.FirstOrDefault(m => m.UserId == currentUserId);
        if (currentUserMember is null)
        {
            return BadRequest(new ErrorResponse("You are not a member of this group."));
        }

        // Validar se usuario informado no accesstoken é dono do grupo
        if (!currentUserMember.IsOwner)
        {
            return BadRequest(new ErrorResponse("Only group owner can remove members."));
        }

        // Validar se usuario informado na request é membro do grupo
        var userToRemove = group.Members.FirstOrDefault(m => m.UserId == request.UserId);
        if (userToRemove is null)
        {
            return NotFound(new ErrorResponse("User is not a member of this group."));
        }

        // Remover usuario informado na request do grupo
        groupMemberRepository.Remove(userToRemove);
        await unitOfWork.SaveAsync();

        return NoContent();
    }
}