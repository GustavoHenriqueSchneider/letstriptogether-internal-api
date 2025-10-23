using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Context.Interfaces;
using WebApi.DTOs.Responses;
using WebApi.Models;
using WebApi.Persistence.Interfaces;
using WebApi.Repositories.Interfaces;

namespace WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/group-matches")]
public class GroupMatchController(
    IGroupMatchRepository groupMatchRepository,
    IGroupRepository groupRepository,
    IApplicationUserContext currentUser,
    IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> RemoveGroupMatch([FromRoute] Guid id)
    {
        var currentUserId = currentUser.GetId();

        // Buscar o match com as relações para verificar se o usuário é membro do grupo
        var groupMatch = await groupMatchRepository.GetByIdWithRelationsAsync(id);

        if (groupMatch is null)
        {
            return NotFound(new ErrorResponse("Group match not found."));
        }

        // Verificar se o usuário é membro do grupo
        var group = await groupRepository.GetGroupWithMembersAsync(groupMatch.GroupId);
        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var userMember = group.Members.SingleOrDefault(m => m.UserId == currentUserId);
        if (userMember is null)
        {
            return BadRequest(new ErrorResponse("You are not a member of this group."));
        }

        // Apenas o owner do grupo pode remover matches
        if (!userMember.IsOwner)
        {
            return BadRequest(new ErrorResponse("Only the group owner can remove matches."));
        }

        groupMatchRepository.Remove(groupMatch);
        await unitOfWork.SaveAsync();

        return NoContent();
    }
}
