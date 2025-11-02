using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs.Responses;
using WebApi.DTOs.Responses.Admin.Group;
using WebApi.Repositories.Interfaces;
using WebApi.Security;

namespace WebApi.Controllers.Admin;

// TODO: aplicar CQRS com usecases, mediator com mediatr e clean arc
// TODO: colocar tag de versionamento e descricoes para swagger
// TODO: converter returns de erro em exception

[ApiController]
[Authorize(Policy = Policies.Admin)]
[Route("api/v1/admin/groups")]
[Tags("Admin - Grupos")]
public class AdminGroupController(
    IGroupRepository groupRepository) : ControllerBase
{
    /// <summary>
    ///  Busca todos os grupos (Admin).
    /// </summary>
    /// <remarks>
    /// Retorna uma lista de todos os grupos ordenado por pagina��o.
    /// </remarks>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <response code="200">Retorna lista paginada de todos os grupos</response>
    /// <response code="401">Usu�rio n�o autorizado(Token inv�lido ou vencido)</response>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(AdminGetAllGroupsResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AdminGetAllGroups([FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10)
    {
        var (groups, hits) = await groupRepository.GetAllAsync(pageNumber, pageSize);

        return Ok(new AdminGetAllGroupsResponse
        {
            Data = groups.Select(x => new AdminGetAllGroupsResponseData
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt
            }),
            Hits = hits
        });
    }
    /// <summary>
    /// Busca um grupo pelo Id (Admin).
    /// </summary>
    /// <param name="groupId">Retorna o Guid do grupo a ser buscado</param>
    /// <response code="200">Retorna o grupo buscado pelo Id</response>
    /// <response code="401">Usu�rio n�o autorizado(Token inv�lido ou vencido)</response>
    /// <response code="404">Grupo n�o encontrado</response>
    [HttpGet("{groupId:guid}")]
    [ProducesResponseType(typeof(AdminGetGroupByIdResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdminGetGroupById([FromRoute] Guid groupId)
    {
        var group = await groupRepository.GetByIdAsync(groupId);

        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        return Ok(new AdminGetGroupByIdResponse
        {
            Name = group.Name,
            TripExpectedDate = group.TripExpectedDate,
            CreatedAt = group.CreatedAt,
            UpdatedAt = group.UpdatedAt
        });
    }
}

