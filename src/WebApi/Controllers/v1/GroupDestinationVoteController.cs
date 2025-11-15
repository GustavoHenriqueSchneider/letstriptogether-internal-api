using Application.Common.Interfaces.Extensions;
using Application.UseCases.v1.GroupDestinationVote.Command.UpdateDestinationVoteById;
using Application.UseCases.v1.GroupDestinationVote.Command.VoteAtDestinationForGroupId;
using Application.UseCases.v1.GroupDestinationVote.Query.GetGroupDestinationVoteById;
using Application.UseCases.v1.GroupDestinationVote.Query.GetGroupMemberAllDestinationVotesById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers.v1;

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/groups/{groupId:guid}/destination-votes")]
public class GroupDestinationVoteController(
    IMediator mediator,
    IApplicationUserContextExtensions currentUser) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(
        Summary = "Votar em Destino",
        Description = "Registra ou atualiza o voto do usuário autenticado em um destino do grupo.")]
    [ProducesResponseType(typeof(VoteAtDestinationForGroupIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> VoteAtDestinationForGroupId([FromRoute] Guid groupId,
        [FromBody] VoteAtDestinationForGroupIdCommand command, CancellationToken cancellationToken)
    {
        command = command with
        {
            GroupId = groupId,
            UserId = currentUser.GetId()
        };

        var response = await mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{destinationVoteId:guid}")]
    [SwaggerOperation(
        Summary = "Atualizar Voto em Destino",
        Description = "Atualiza um voto existente do usuário autenticado em um destino do grupo.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDestinationVoteById(
        [FromRoute] Guid groupId, 
        [FromRoute] Guid destinationVoteId, 
        [FromBody] UpdateDestinationVoteByIdCommand command, 
        CancellationToken cancellationToken)
    {
        command = command with
        {
            GroupId = groupId,
            DestinationVoteId = destinationVoteId,
            UserId = currentUser.GetId()
        };

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Listar Todos os Votos em Destinos do Usuário",
        Description = "Retorna uma lista paginada de todos os votos em destinos do usuário autenticado no grupo especificado.")]
    [ProducesResponseType(typeof(GetGroupMemberAllDestinationVotesByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGroupMemberAllDestinationVotesById([FromRoute] Guid groupId,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new GetGroupMemberAllDestinationVotesByIdQuery
        {
            GroupId = groupId,
            UserId = currentUser.GetId(),
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{destinationVoteId:guid}")]
    [SwaggerOperation(
        Summary = "Obter Voto em Destino por ID",
        Description = "Retorna os detalhes de um voto específico em destino do grupo.")]
    [ProducesResponseType(typeof(GetGroupDestinationVoteByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGroupDestinationVoteById(
        [FromRoute] Guid groupId, 
        [FromRoute] Guid destinationVoteId, 
        CancellationToken cancellationToken)
    {
        var query = new GetGroupDestinationVoteByIdQuery
        {
            GroupId = groupId,
            DestinationVoteId = destinationVoteId,
            UserId = currentUser.GetId()
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }
}
