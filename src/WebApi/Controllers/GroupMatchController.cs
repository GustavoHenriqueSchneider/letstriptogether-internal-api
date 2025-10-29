using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using WebApi.Context.Interfaces;
using WebApi.DTOs.Responses;
using WebApi.Models;
using WebApi.Persistence.Interfaces;
using WebApi.Repositories.Implementations;
using WebApi.Repositories.Interfaces;

namespace WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/groups/{groupId:guid}/matches")]
public class GroupMatchController(
    IGroupMatchRepository groupMatchRepository,
    IGroupRepository groupRepository,
    IApplicationUserContext currentUser,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpDelete("{matchId:guid}")]
    public async Task<IActionResult> RemoveGroupMatchById([FromRoute] Guid groupId,
        [FromRoute] Guid matchId)
    {
        var currentUserId = currentUser.GetId();

        if (!await userRepository.ExistsByIdAsync(currentUserId))
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var group = await groupRepository.GetGroupWithMatchesAsync(groupId);

        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var isMember = await groupRepository.IsGroupMemberByUserIdAsync(groupId, currentUserId);

        if (!isMember)
        {
            return BadRequest(new ErrorResponse("You are not a member of this group."));
        }

        var matchToRemove = group.Matches.SingleOrDefault(m => m.Id == matchId);

        if (matchToRemove is null)
        {
            return NotFound(new ErrorResponse("The match was not found for this group."));
        }

        groupMatchRepository.Remove(matchToRemove);
        await unitOfWork.SaveAsync();

        return NoContent();
    }
}
