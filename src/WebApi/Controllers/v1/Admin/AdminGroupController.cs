using Application.Common.Policies;
using Application.UseCases.Admin.AdminGroup.Query.AdminGetAllGroups;
using Application.UseCases.Admin.AdminGroup.Query.AdminGetGroupById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers.v1.Admin;

// TODO: descricoes para swagger

[ApiController]
[Authorize(Policy = Policies.Admin)]
[Route("api/v{version:apiVersion}/admin/groups")]
public class AdminGroupController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> AdminGetAllGroups([FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new AdminGetAllGroupsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{groupId:guid}")]
    [SwaggerOperation(
        Summary = "Obter Grupo por ID (Admin)",
        Description = "Retorna os detalhes de um grupo específico. Requer permissões de administrador.")]
    [ProducesResponseType(typeof(AdminGetGroupByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdminGetGroupById(
        [FromRoute] Guid groupId, 
        CancellationToken cancellationToken)
    {
        var query = new AdminGetGroupByIdQuery
        {
            GroupId = groupId
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }
}
