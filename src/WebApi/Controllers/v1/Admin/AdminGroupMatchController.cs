using Application.Common.Policies;
using Application.UseCases.Admin.AdminGroupMatch.Query.AdminGetAllGroupMatchesById;
using Application.UseCases.Admin.AdminGroupMatch.Query.AdminGetGroupMatchById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers.v1.Admin;

[ApiController]
[Authorize(Policy = Policies.Admin)]
[Route("api/v{version:apiVersion}/admin/groups/{groupId:guid}/matches")]
public class AdminGroupMatchController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(
        Summary = "Listar Todos os Matches do Grupo (Admin)",
        Description = "Retorna uma lista paginada de todos os matches (destinos compatíveis) de um grupo específico. Requer permissões de administrador.")]
    [ProducesResponseType(typeof(AdminGetAllGroupMatchesByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdminGetAllGroupMatchesById([FromRoute] Guid groupId,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new AdminGetAllGroupMatchesByIdQuery
        {
            GroupId = groupId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{matchId:guid}")]
    [SwaggerOperation(
        Summary = "Obter Match do Grupo por ID (Admin)",
        Description = "Retorna os detalhes de um match específico de um grupo. Requer permissões de administrador.")]
    [ProducesResponseType(typeof(AdminGetGroupMatchByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdminGetGroupMatchById(
        [FromRoute] Guid groupId,
        [FromRoute] Guid matchId, 
        CancellationToken cancellationToken)
    {
        var query = new AdminGetGroupMatchByIdQuery
        {
            GroupId = groupId,
            MatchId = matchId
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }
}
