using Application.Common.Policies;
using Application.UseCases.Admin.AdminGroupDestinationVote.Query.AdminGetAllGroupDestinationVotesById;
using Application.UseCases.Admin.AdminGroupDestinationVote.Query.AdminGetGroupDestinationVoteById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers.v1.Admin;

[ApiController]
[Authorize(Policy = Policies.Admin)]
[Route("api/v{version:apiVersion}/admin/groups/{groupId:guid}/destination-votes")]
public class AdminGroupDestinationVoteController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(
        Summary = "Listar Todos os Votos em Destinos do Grupo (Admin)",
        Description = "Retorna uma lista paginada de todos os votos em destinos de um grupo específico. Requer permissões de administrador.")]
    [ProducesResponseType(typeof(AdminGetAllGroupDestinationVotesByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdminGetAllGroupDestinationVotesById(
        [FromRoute] Guid groupId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new AdminGetAllGroupDestinationVotesByIdQuery
        {
            GroupId = groupId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{destinationVoteId:guid}")]
    [SwaggerOperation(
        Summary = "Obter Voto em Destino do Grupo por ID (Admin)",
        Description = "Retorna os detalhes de um voto específico em destino de um grupo. Requer permissões de administrador.")]
    [ProducesResponseType(typeof(AdminGetGroupDestinationVoteByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdminGetGroupDestinationVoteById(
        [FromRoute] Guid groupId, 
        [FromRoute] Guid destinationVoteId, 
        CancellationToken cancellationToken)
    {
        var query = new AdminGetGroupDestinationVoteByIdQuery
        {
            GroupId = groupId,
            DestinationVoteId = destinationVoteId
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }
}
