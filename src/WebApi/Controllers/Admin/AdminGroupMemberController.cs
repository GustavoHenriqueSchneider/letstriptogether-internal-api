using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs.Responses;
using WebApi.DTOs.Responses.Admin.GroupMember;
using WebApi.Persistence.Interfaces;
using WebApi.Repositories.Interfaces;
using WebApi.Security;

namespace WebApi.Controllers.Admin;

[ApiController]
[Authorize(Policy = Policies.Admin)]
[Route("api/v1/admin/groups/{groupId:guid}/members")]
[Tags("Admin - Membros")]
public class AdminGroupMemberController(
    IUnitOfWork unitOfWork,
    IGroupRepository groupRepository,
    IGroupMemberDestinationVoteRepository groupMemberDestinationVoteRepository,
    IGroupMemberRepository groupMemberRepository) : ControllerBase
{
    /// <summary>
    ///  Busca todos os membros de um grupo (Admin).
    /// </summary>
    /// <remarks>
    /// Retorna uma lista de todos os membros do grupo ordenado por paginação.
    /// </remarks>
    /// <param name="groupId"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <response code="200">Retorna lista paginada de todos os membros do grupo</response>
    /// <response code="401">Usuário não autorizado(Token inválido ou vencido)</response>
    /// <response code="404">Grupo não encontrado</response>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(AdminGetAllGroupMembersByIdResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdminGetAllGroupMembersById([FromRoute] Guid groupId,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var groupExists = await groupRepository.ExistsByIdAsync(groupId);

        if (!groupExists)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var (groupMembers, hits) =
            await groupMemberRepository.GetAllByGroupIdAsync(groupId, pageNumber, pageSize);

        return Ok(new AdminGetAllGroupMembersByIdResponse
        {
            Data = groupMembers.Select(x => new AdminGetAllGroupMembersByIdResponseData
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt
            }),
            Hits = hits
        });
    }
    /// <summary>
    /// Busca um membro de grupo pelo Id (Admin).
    /// </summary>
    /// <param name="groupId"></param>
    /// <param name="memberId">Retorna o Guid do membro a ser buscado</param>
    /// <response code="200">Retorna o membro buscado pelo Id</response>
    /// <response code="401">Usuário não autorizado(Token inválido ou vencido)</response>
    /// <response code="404">Membro ou grupo não encontrado</response>
    [HttpGet("{memberId:guid}")]
    [ProducesResponseType(typeof(AdminGetGroupMemberByIdResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdminGetGroupMemberById([FromRoute] Guid groupId, 
        [FromRoute] Guid memberId)
    {
        var group = await groupRepository.GetGroupWithMembersAsync(groupId);

        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var groupMember = group.Members.SingleOrDefault(x => x.Id == memberId);

        if (groupMember is null)
        {
            return NotFound(new ErrorResponse("Group member not found."));
        }

        return Ok(new AdminGetGroupMemberByIdResponse
        {
            UserId = groupMember.UserId,
            IsOwner = groupMember.IsOwner,
            CreatedAt = groupMember.CreatedAt,
            UpdatedAt = groupMember.UpdatedAt
        });
    }
    /// <summary>
    /// Remove um membro de grupo pelo Id (Admin).
    /// </summary>
    /// <param name="groupId"></param>
    /// <param name="memberId"></param>
    /// <response code="204">Membro removido com sucesso</response>
    /// <response code="400">Não é possível remover o owner do grupo ou requisição inválida</response>
    /// <response code="401">Usuário não autorizado(Token inválido ou vencido)</response>
    /// <response code="404">Membro ou grupo não encontrado</response>
    [HttpDelete("{memberId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdminRemoveGroupMemberById([FromRoute] Guid groupId,
        [FromRoute] Guid memberId)
    {
        var group = await groupRepository.GetGroupWithMembersAsync(groupId);

        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var userToRemove = group.Members.SingleOrDefault(m => m.Id == memberId);

        if (userToRemove is null)
        {
            return NotFound(new ErrorResponse("The user is not a member of this group."));
        }

        if (userToRemove.IsOwner)
        {
            return BadRequest(new ErrorResponse("It is not possible to remove the owner of group."));
        }

        groupMemberRepository.Remove(userToRemove);
        await unitOfWork.SaveAsync();

        return NoContent();
    }
    /// <summary>
    /// Busca todos os votos de destino de um membro (Admin).
    /// </summary>
    /// <param name="groupId"></param>
    /// <param name="memberId"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <response code="200">Retorna lista paginada de todos os votos do membro</response>
    /// <response code="401">Usuário não autorizado(Token inválido ou vencido)</response>
    /// <response code="404">Membro ou grupo não encontrado</response>
    [HttpGet("{memberId:guid}/destination-votes")]
    [ProducesResponseType(typeof(AdminGetGroupMemberAllDestinationVotesByIdResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdminGetGroupMemberAllDestinationVotesById([FromRoute] Guid groupId,
        [FromRoute] Guid memberId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var group = await groupRepository.GetGroupWithMembersAsync(groupId);

        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var isGroupMember = group.Members.Any(m => m.Id == memberId);

        if (!isGroupMember)
        {
            return NotFound(new ErrorResponse("The user is not a member of this group."));
        }

        var (votes, hits) = await groupMemberDestinationVoteRepository.GetByMemberIdAsync(memberId,
            pageNumber, pageSize);

        return Ok(new AdminGetGroupMemberAllDestinationVotesByIdResponse
        {
            Data = votes.Select(x => new AdminGetGroupMemberAllDestinationVotesByIdResponseData
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt
            }),
            Hits = hits
        });
    }
}