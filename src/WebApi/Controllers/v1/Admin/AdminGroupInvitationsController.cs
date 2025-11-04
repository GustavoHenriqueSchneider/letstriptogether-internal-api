using LetsTripTogether.InternalApi.Application.Common.Policies;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses;
using LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses.Admin.GroupInvitation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetsTripTogether.InternalApi.WebApi.Controllers.v1.Admin;

// TODO: aplicar CQRS com usecases, mediator com mediatr e clean arc
// TODO: colocar tag de versionamento e descricoes para swagger
// TODO: converter returns de erro em exception

[ApiController]
[Authorize(Policy = Policies.Admin)]
[Route("api/v{version:apiVersion}/admin/groups/{groupId:guid}/invitations")]
public class AdminGroupInvitationsController(
    IGroupRepository groupRepository,
    IGroupInvitationRepository groupInvitationRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> AdminGetAllGroupInvitationsByGroupId(
        [FromRoute] Guid groupId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var groupExists = await groupRepository.ExistsByIdAsync(groupId);

        if (!groupExists)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var (invitations, hits) = 
            await groupInvitationRepository.GetByGroupIdAsync(groupId, pageNumber, pageSize);

        return Ok(new AdminGetAllGroupInvitationsByGroupIdResponse
        {
            Data = invitations.Select(x => new AdminGetAllGroupInvitationsByGroupIdResponseData
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt
            }),
            Hits = hits
        });
    }

    [HttpGet("{invitationId:guid}")]
    public async Task<IActionResult> AdminGetGroupInvitationById(
        [FromRoute] Guid groupId, [FromRoute] Guid invitationId)
    {
        var groupExists = await groupRepository.ExistsByIdAsync(groupId);

        if (!groupExists)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var invitation = await groupInvitationRepository.GetByIdAsync(invitationId);

        if (invitation is null)
        {
            return NotFound(new ErrorResponse("Group invitation not found."));
        }

        if (invitation.GroupId != groupId)
        {
            return BadRequest(new ErrorResponse("The invitation does not belong to this group."));
        }

        return Ok(new AdminGetGroupInvitationByIdResponse
        {
            Status = invitation.Status.ToString(),
            ExpirationDate = invitation.ExpirationDate,
            CreatedAt = invitation.CreatedAt,
            UpdatedAt = invitation.UpdatedAt
        });
    }
}

