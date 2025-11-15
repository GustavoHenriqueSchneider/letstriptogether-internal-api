using Application.Common.Interfaces.Extensions;
using Application.UseCases.v1.Group.Command.CreateGroup;
using Application.UseCases.v1.Group.Command.DeleteGroupById;
using Application.UseCases.v1.Group.Command.LeaveGroupById;
using Application.UseCases.v1.Group.Command.UpdateGroupById;
using Application.UseCases.v1.Group.Query.GetAllGroups;
using Application.UseCases.v1.Group.Query.GetGroupById;
using Application.UseCases.v1.Group.Query.GetNotVotedDestinationsByMemberOnGroup;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers.v1;

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/groups")]
public class GroupController(
    IMediator mediator,
    IApplicationUserContextExtensions currentUser) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(
        Summary = "Criar Grupo",
        Description = "Cria um novo grupo de viagem associado ao usuário autenticado. O usuário deve ter preferências preenchidas para criar um grupo.")]
    [ProducesResponseType(typeof(CreateGroupResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupCommand command, CancellationToken cancellationToken)
    {
        command = command with
        {
            UserId = currentUser.GetId()
        };

        var response = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(CreateGroup), response);
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Listar Todos os Grupos",
        Description = "Retorna uma lista paginada de todos os grupos do usuário autenticado.")]
    [ProducesResponseType(typeof(GetAllGroupsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllGroups(
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10, 
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllGroupsQuery
        {
            UserId = currentUser.GetId(),
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{groupId:guid}")]
    [SwaggerOperation(
        Summary = "Obter Grupo por ID",
        Description = "Retorna os detalhes de um grupo específico, incluindo membros, preferências e matches.")]
    [ProducesResponseType(typeof(GetGroupByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGroupById([FromRoute] Guid groupId, CancellationToken cancellationToken)
    {
        var query = new GetGroupByIdQuery
        {
            GroupId = groupId,
            UserId = currentUser.GetId()
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{groupId:guid}")]
    [SwaggerOperation(
        Summary = "Atualizar Grupo",
        Description = "Atualiza as informações de um grupo existente. Apenas o proprietário do grupo pode atualizar.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateGroupById(
        [FromRoute] Guid groupId, 
        [FromBody] UpdateGroupByIdCommand command, 
        CancellationToken cancellationToken)
    {
        command = command with
        {
            GroupId = groupId,
            UserId = currentUser.GetId()
        };

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{groupId:guid}")]
    [SwaggerOperation(
        Summary = "Excluir Grupo",
        Description = "Exclui permanentemente um grupo. Apenas o proprietário do grupo pode excluir.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteGroupById([FromRoute] Guid groupId, CancellationToken cancellationToken)
    {
        var command = new DeleteGroupByIdCommand
        {
            GroupId = groupId,
            UserId = currentUser.GetId()
        };

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
    
    [HttpPatch("{groupId:guid}/leave")]
    [SwaggerOperation(
        Summary = "Sair do Grupo",
        Description = "Remove o usuário autenticado de um grupo. Se o usuário for o proprietário, o grupo será excluído.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> LeaveGroupById([FromRoute] Guid groupId, CancellationToken cancellationToken)
    {
        var command = new LeaveGroupByIdCommand
        {
            GroupId = groupId,
            UserId = currentUser.GetId()
        };

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
    
    [HttpGet("{groupId:guid}/destinations-not-voted")]
    [SwaggerOperation(
        Summary = "Obter Destinos Não Votados",
        Description = "Retorna uma lista paginada de destinos que o usuário ainda não votou no grupo especificado.")]
    [ProducesResponseType(typeof(GetNotVotedDestinationsByMemberOnGroupResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetNotVotedDestinationsByMemberOnGroup(
        [FromRoute] Guid groupId,
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10, 
        CancellationToken cancellationToken = default)
    {
        var query = new GetNotVotedDestinationsByMemberOnGroupQuery
        {
            GroupId = groupId,
            UserId = currentUser.GetId(),
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }
}
