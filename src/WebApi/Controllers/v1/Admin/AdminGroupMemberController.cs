using Application.Common.Policies;
using Application.UseCases.Admin.AdminGroupMember.Command.AdminRemoveGroupMemberById;
using Application.UseCases.Admin.AdminGroupMember.Query.AdminGetAllGroupMembersById;
using Application.UseCases.Admin.AdminGroupMember.Query.AdminGetGroupMemberAllDestinationVotesById;
using Application.UseCases.Admin.AdminGroupMember.Query.AdminGetGroupMemberById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers.v1.Admin;

[ApiController]
[Authorize(Policy = Policies.Admin)]
[Route("api/v{version:apiVersion}/admin/groups/{groupId:guid}/members")]
public class AdminGroupMemberController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(
        Summary = "Listar Todos os Membros do Grupo (Admin)",
        Description = "Retorna uma lista paginada de todos os membros de um grupo específico. Requer permissões de administrador.")]
    [ProducesResponseType(typeof(AdminGetAllGroupMembersByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdminGetAllGroupMembersById([FromRoute] Guid groupId,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new AdminGetAllGroupMembersByIdQuery
        {
            GroupId = groupId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{memberId:guid}")]
    [SwaggerOperation(
        Summary = "Obter Membro do Grupo por ID (Admin)",
        Description = "Retorna os detalhes de um membro específico de um grupo. Requer permissões de administrador.")]
    [ProducesResponseType(typeof(AdminGetGroupMemberByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdminGetGroupMemberById([FromRoute] Guid groupId, 
        [FromRoute] Guid memberId, CancellationToken cancellationToken)
    {
        var query = new AdminGetGroupMemberByIdQuery
        {
            GroupId = groupId,
            MemberId = memberId
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{memberId:guid}")]
    [SwaggerOperation(
        Summary = "Remover Membro do Grupo (Admin)",
        Description = "Remove um membro de um grupo. Requer permissões de administrador.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdminRemoveGroupMemberById(
        [FromRoute] Guid groupId,
        [FromRoute] Guid memberId, 
        CancellationToken cancellationToken)
    {
        var command = new AdminRemoveGroupMemberByIdCommand
        {
            GroupId = groupId,
            MemberId = memberId
        };

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpGet("{memberId:guid}/destination-votes")]
    [SwaggerOperation(
        Summary = "Listar Todos os Votos em Destinos do Membro (Admin)",
        Description = "Retorna uma lista paginada de todos os votos em destinos de um membro específico em um grupo. Requer permissões de administrador.")]
    [ProducesResponseType(typeof(AdminGetGroupMemberAllDestinationVotesByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdminGetGroupMemberAllDestinationVotesById(
        [FromRoute] Guid groupId,
        [FromRoute] Guid memberId, 
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10, 
        CancellationToken cancellationToken = default)
    {
        var query = new AdminGetGroupMemberAllDestinationVotesByIdQuery
        {
            GroupId = groupId,
            MemberId = memberId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }
}
