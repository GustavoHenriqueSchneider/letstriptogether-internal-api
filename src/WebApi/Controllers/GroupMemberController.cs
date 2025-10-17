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
[Route("api/v1/groups/{groupId:guid}/members")]
public class GroupMemberController(
    IUnitOfWork unitOfWork,
    IApplicationUserContext currentUser,
    IUserRepository userRepository,
    IGroupRepository groupRepository,
    IGroupMemberRepository groupMemberRepository) : ControllerBase
{
    [HttpDelete]
    public async Task<IActionResult> RemoveUserFromGroup(
        [FromRoute] Guid groupId, 
        [FromBody] RemoveUserFromGroupRequest request)
    {
        var currentUserId = currentUser.GetId();
        var currentUserEntity = await userRepository.GetByIdAsync(currentUserId);
        if (currentUserEntity is null)
        {
            return NotFound(new ErrorResponse("Usu�rio n�o encontrado."));
        }
        var group = await groupRepository.GetGroupWithMembersAsync(groupId);
        if (group is null)
        {
            return NotFound(new ErrorResponse("Grupo n�o encontrado."));
        }
        var currentUserMember = group.Members.SingleOrDefault(m => m.UserId == currentUserId);
        if (currentUserMember is null)
        {
            return BadRequest(new ErrorResponse("Voc� n�o � membro desse grupo."));
        }
        if (!currentUserMember.IsOwner)
        {
            return BadRequest(new ErrorResponse("S� o dono do grupo pode remover membros."));
        }
        var userToRemove = group.Members.SingleOrDefault(m => m.UserId == request.UserId);
        if (userToRemove is null)
        {
            return NotFound(new ErrorResponse("O usu�rio n�o � membro desse grupo."));
        }
        groupMemberRepository.Remove(userToRemove);
        await unitOfWork.SaveAsync();

        return NoContent();
    }
}