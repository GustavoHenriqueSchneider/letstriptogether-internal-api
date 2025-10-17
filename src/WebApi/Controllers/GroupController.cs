using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs.Responses;
using WebApi.DTOs.Responses.Group;
using WebApi.Repositories.Interfaces;
using WebApi.Security;

namespace WebApi.Controllers;

// TODO: aplicar CQRS com usecases, mediator com mediatr e clean arc
// TODO: colocar tag de versionamento e descricoes para swagger
// TODO: converter returns de erro em exception

[Authorize]
[ApiController]
[Route("api/v1/groups")]
public class GroupController(IGroupRepository groupRepository, IGroupMemberRepository groupMemberRepository) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = Policies.Admin)]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10)
    {
        var (groups, hits) = await groupRepository.GetAllAsync(pageNumber, pageSize);

        return Ok(new GetAllGroupsResponse
        {
            Data = groups.Select(x => new GetAllGroupsResponseData
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt
            }),
            Hits = hits
        });
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = Policies.Admin)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var group = await groupRepository.GetByIdAsync(id);

        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        return Ok(new GetGroupByIdResponse
        {
            Name = group.Name,
            TripExpectedDate = group.TripExpectedDate,
            CreatedAt = group.CreatedAt,
            UpdatedAt = group.UpdatedAt
        });
    }
    [HttpDelete("{groupId:guid}/members/{userId:guid}")]
    public async Task<IActionResult> DeleteMember([FromRoute] Guid groupId, [FromRoute] Guid userId)
    {
        // 1. Encontra a entidade que liga o grupo ao usuário
        var memberToRemove = await groupMemberRepository.FindMemberAsync(groupId, userId);

        // 2. Se a ligação não existe, retorna erro 404
        if (memberToRemove is null)
        {
            return NotFound(new ErrorResponse("Member not found in this group."));
        }

        // 3. Deleta a ligação e salva no banco de dados
        await groupMemberRepository.DeleteAsync(memberToRemove);

        // 4. Retorna sucesso (204 No Content é o padrão para DELETE)
        return NoContent();
    }
}

