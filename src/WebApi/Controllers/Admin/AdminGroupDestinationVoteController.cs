using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs.Responses;
using WebApi.DTOs.Responses.Admin.GroupMemberDestinationVote;
using WebApi.Repositories.Interfaces;
using WebApi.Security;

namespace WebApi.Controllers.Admin;

[ApiController]
[Authorize(Policy = Policies.Admin)]
[Route("api/v1/admin/groups/{groupId:guid}/destination-votes")]
[Tags("Admin - Votos")]
public class AdminGroupDestinationVoteController(
    IGroupRepository groupRepository,
    IGroupMemberDestinationVoteRepository groupMemberDestinationVoteRepository) : ControllerBase
{
    /// <summary>
    ///  Busca todos os votos de destino de um grupo (Admin).
    /// </summary>
    /// <remarks>
    /// Retorna uma lista de todos os votos de destino do grupo ordenado por paginação.
    /// </remarks>
    /// <param name="groupId"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <response code="200">Retorna lista paginada de todos os votos de destino do grupo</response>
    /// <response code="401">Usuário não autorizado(Token inválido ou vencido)</response>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(AdminGetAllGroupDestinationVotesByIdResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AdminGetAllGroupDestinationVotesById(
        [FromRoute] Guid groupId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var (votes, hits) = 
            await groupMemberDestinationVoteRepository.GetByGroupIdAsync(groupId, 
                pageNumber, pageSize);

        return Ok(new AdminGetAllGroupDestinationVotesByIdResponse
        {
            Data = votes.Select(x => new AdminGetAllGroupDestinationVotesByIdResponseData
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt
            }),
            Hits = hits
        });
    }
    /// <summary>
    /// Busca um voto de destino de grupo pelo Id (Admin).
    /// </summary>
    /// <param name="groupId"></param>
    /// <param name="destinationVoteId">Retorna o Guid do voto a ser buscado</param>
    /// <response code="200">Retorna o voto buscado pelo Id</response>
    /// <response code="401">Usuário não autorizado(Token inválido ou vencido)</response>
    /// <response code="404">Voto ou grupo não encontrado</response>
    [HttpGet("{destinationVoteId:guid}")]
    [ProducesResponseType(typeof(AdminGetGroupDestinationVoteByIdResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdminGetGroupDestinationVoteById(
        [FromRoute] Guid groupId, [FromRoute] Guid destinationVoteId)
    {
        var groupExists = await groupRepository.ExistsByIdAsync(groupId);

        if (!groupExists)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var vote = await groupMemberDestinationVoteRepository.GetByIdWithRelationsAsync(groupId, 
            destinationVoteId);

        if (vote is null)
        {
            return NotFound(new ErrorResponse("Group member destination vote not found."));
        }

        return Ok(new AdminGetGroupDestinationVoteByIdResponse
        {
            GroupId = vote.GroupMember.GroupId,
            MemberId = vote.GroupMemberId,
            DestinationId = vote.DestinationId,
            IsApproved = vote.IsApproved,
            CreatedAt = vote.CreatedAt,
            UpdatedAt = vote.UpdatedAt
        });
    }
}
