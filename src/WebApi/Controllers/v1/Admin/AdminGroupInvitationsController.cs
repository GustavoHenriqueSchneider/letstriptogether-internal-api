using Application.Common.Policies;
using Application.UseCases.Admin.AdminGroupInvitation.Query.AdminGetAllGroupInvitationsByGroupId;
using Application.UseCases.Admin.AdminGroupInvitation.Query.AdminGetGroupInvitationById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers.v1.Admin;

// TODO: descricoes para swagger

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
    [SwaggerOperation(
        Summary = "Obter Convite do Grupo por ID (Admin)",
        Description = "Retorna os detalhes de um convite específico de um grupo. Requer permissões de administrador.")]
    [ProducesResponseType(typeof(AdminGetGroupInvitationByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdminGetGroupInvitationById(
        [FromRoute] Guid groupId, 
        [FromRoute] Guid invitationId, 
        CancellationToken cancellationToken)
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
