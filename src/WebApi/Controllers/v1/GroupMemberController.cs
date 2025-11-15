using Application.Common.Interfaces.Extensions;
using Application.UseCases.v1.GroupMember.Command.RemoveGroupMemberById;
using Application.UseCases.v1.GroupMember.Query.GetGroupMemberById;
using Application.UseCases.v1.GroupMember.Query.GetOtherGroupMembersById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers.v1;

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/groups/{groupId:guid}/members")]
public class GroupMemberController(
    IMediator mediator,
    IApplicationUserContextExtensions currentUser) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(
        Summary = "Listar Outros Membros do Grupo",
        Description = "Retorna uma lista paginada de todos os membros do grupo, excluindo o usuário autenticado.")]
    [ProducesResponseType(typeof(GetOtherGroupMembersByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOtherGroupMembersById([FromRoute] Guid groupId, 
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new GetOtherGroupMembersByIdQuery
        {
            GroupId = groupId,
            UserId = currentUser.GetId(),
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{memberId:guid}")]
    [SwaggerOperation(
        Summary = "Obter Membro do Grupo por ID",
        Description = "Retorna os detalhes de um membro específico do grupo.")]
    [ProducesResponseType(typeof(GetGroupMemberByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGroupMemberById([FromRoute] Guid groupId, 
        [FromRoute] Guid memberId, CancellationToken cancellationToken)
    {
        var query = new GetGroupMemberByIdQuery
        {
            GroupId = groupId,
            MemberId = memberId,
            UserId = currentUser.GetId()
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{memberId:guid}")]
    [SwaggerOperation(
        Summary = "Remover Membro do Grupo",
        Description = "Remove um membro do grupo. Apenas o proprietário do grupo pode remover membros.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveGroupMemberById(
        [FromRoute] Guid groupId,
        [FromRoute] Guid memberId, 
        CancellationToken cancellationToken)
    {
        var command = new RemoveGroupMemberByIdCommand
        {
            GroupId = groupId,
            MemberId = memberId,
            UserId = currentUser.GetId()
        };

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
