using LetsTripTogether.InternalApi.Application.Common.Policies;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupInvitation.Query.AdminGetAllGroupInvitationsByGroupId;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupInvitation.Query.AdminGetGroupInvitationById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetsTripTogether.InternalApi.WebApi.Controllers.v1.Admin;

// TODO: colocar tag de versionamento e descricoes para swagger

[ApiController]
[Authorize(Policy = Policies.Admin)]
[Route("api/v{version:apiVersion}/admin/groups/{groupId:guid}/invitations")]
public class AdminGroupInvitationsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> AdminGetAllGroupInvitationsByGroupId(
        [FromRoute] Guid groupId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new AdminGetAllGroupInvitationsByGroupIdQuery
        {
            GroupId = groupId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{invitationId:guid}")]
    public async Task<IActionResult> AdminGetGroupInvitationById(
        [FromRoute] Guid groupId, [FromRoute] Guid invitationId, CancellationToken cancellationToken)
    {
        var query = new AdminGetGroupInvitationByIdQuery
        {
            GroupId = groupId,
            InvitationId = invitationId
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }
}
