using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs.Responses;
using WebApi.DTOs.Responses.Admin.GroupMatch;
using WebApi.Repositories.Interfaces;
using WebApi.Security;

namespace WebApi.Controllers.Admin;

[ApiController]
[Authorize(Policy = Policies.Admin)]
[Route("api/v1/admin/groups/{groupId:guid}/matches")]
[Tags("Admin - Matches")]
public class AdminGroupMatchController(
    IGroupMatchRepository groupMatchRepository,
    IGroupRepository groupRepository) : ControllerBase
{
    /// <summary>
    ///  Busca todos os matches de um grupo (Admin).
    /// </summary>
    /// <remarks>
    /// Retorna uma lista de todos os matches do grupo ordenado por paginação.
    /// </remarks>
    /// <param name="groupId"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <response code="200">Retorna lista paginada de todos os matches do grupo</response>
    /// <response code="401">Usuário não autorizado(Token inválido ou vencido)</response>
    /// <response code="404">Grupo não encontrado</response>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(AdminGetAllGroupMatchesByIdResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdminGetAllGroupMatchesById([FromRoute] Guid groupId,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var groupExists = await groupRepository.ExistsByIdAsync(groupId);

        if (!groupExists)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var (groupMatches, hits) = 
            await groupMatchRepository.GetByGroupIdAsync(groupId, pageNumber, pageSize);

        return Ok(new AdminGetAllGroupMatchesByIdResponse
        {
            Data = groupMatches.Select(x => new AdminGetAllGroupMatchesByIdResponseData
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt
            }),
            Hits = hits
        });
    }
    /// <summary>
    /// Busca um match de grupo pelo Id (Admin).
    /// </summary>
    /// <param name="groupId"></param>
    /// <param name="matchId">Retorna o Guid do match a ser buscado</param>
    /// <response code="200">Retorna o match buscado pelo Id</response>
    /// <response code="401">Usuário não autorizado(Token inválido ou vencido)</response>
    /// <response code="404">Match ou grupo não encontrado</response>
    [HttpGet("{matchId:guid}")]
    [ProducesResponseType(typeof(AdminGetGroupMatchByIdResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdminGetGroupMatchById([FromRoute] Guid groupId,
        [FromRoute] Guid matchId)
    {
        var group = await groupRepository.GetGroupWithMatchesAsync(groupId);

        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var groupMatch = group.Matches.SingleOrDefault(x => x.Id == matchId);

        if (groupMatch is null)
        {
            return NotFound(new ErrorResponse("Group match not found."));
        }

        return Ok(new AdminGetGroupMatchByIdResponse
        {
            DestinationId = groupMatch.DestinationId,
            CreatedAt = groupMatch.CreatedAt,
            UpdatedAt = groupMatch.UpdatedAt
        });
    }
}
