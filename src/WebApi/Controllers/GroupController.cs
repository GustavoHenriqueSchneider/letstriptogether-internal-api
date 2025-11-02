using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Context.Interfaces;
using WebApi.DTOs.Requests.Group;
using WebApi.DTOs.Responses;
using WebApi.DTOs.Responses.Group;
using WebApi.Models;
using WebApi.Persistence.Interfaces;
using WebApi.Repositories.Interfaces;

namespace WebApi.Controllers;

// TODO: aplicar CQRS com usecases, mediator com mediatr e clean arc
// TODO: colocar tag de versionamento e descricoes para swagger
// TODO: converter returns de erro em exception

[ApiController]
[Authorize]
[Route("api/v1/groups")]
[Tags("Grupos")]
public class GroupController(
    IGroupRepository groupRepository,
    IApplicationUserContext currentUser,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork) : ControllerBase
{
    /// <summary>
    ///  Cria um novo grupo.
    /// </summary>
    /// <remarks>
    /// Cria um grupo e adiciona o usuário atual como owner.
    /// </remarks>
    /// <param name="request"></param>
    /// <response code="201">Grupo criado com sucesso</response>
    /// <response code="400">Requisição inválida</response>
    /// <response code="401">Usuário não autorizado(Token inválido ou vencido)</response>
    /// <response code="404">Usuário não encontrado</response>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(CreateGroupResponse),StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request)
    {
        var currentUserId = currentUser.GetId();
        var existsUser = await userRepository.ExistsByIdAsync(currentUserId);

        if (!existsUser)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var group = new Group(request.Name, request.TripExpectedDate.ToUniversalTime());

        var groupMember = new GroupMember
        {
            GroupId = group.Id,
            UserId = currentUserId,
            IsOwner = true
        };

        group.AddMember(groupMember);
        await groupRepository.AddAsync(group);

        await unitOfWork.SaveAsync();
        return CreatedAtAction(nameof(CreateGroup), new CreateGroupResponse { Id = group.Id });
    }
    /// <summary>
    ///  Busca todos os grupos do usuário atual.
    /// </summary>
    /// <remarks>
    /// Retorna uma lista de todos os grupos do usuário autenticado ordenado por paginação.
    /// </remarks>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <response code="200">Retorna lista paginada de todos os grupos do usuário</response>
    /// <response code="401">Usuário não autorizado(Token inválido ou vencido)</response>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(GetAllGroupsResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllGroups([FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10)
    {
        var (groups, hits) = await groupRepository.GetAllGroupsByUserIdAsync(
            currentUser.GetId(), pageNumber, pageSize);

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
    /// <summary>
    /// Busca um grupo pelo Id.
    /// </summary>
    /// <param name="groupId">Retorna o Guid do grupo a ser buscado</param>
    /// <response code="200">Retorna o grupo buscado pelo Id</response>
    /// <response code="400">Usuário não é membro deste grupo</response>
    /// <response code="401">Usuário não autorizado(Token inválido ou vencido)</response>
    /// <response code="404">Grupo não encontrado</response>
    [HttpGet("{groupId:guid}")]
    [ProducesResponseType(typeof(GetGroupByIdResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGroupById([FromRoute] Guid groupId)
    {
        var group = await groupRepository.GetGroupWithMembersAsync(groupId);

        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var isMember = group.Members.Any(x => x.UserId == currentUser.GetId());

        if (!isMember)
        {
            return BadRequest(new ErrorResponse("You are not a member of this group."));
        }

        return Ok(new GetGroupByIdResponse
        {
            Name = group.Name,
            TripExpectedDate = group.TripExpectedDate,
            CreatedAt = group.CreatedAt,
            UpdatedAt = group.UpdatedAt
        });
    }
    /// <summary>
    /// Atualiza um grupo pelo Id.
    /// </summary>
    /// <param name="groupId"></param>
    /// <param name="request"></param>
    /// <response code="204">Grupo atualizado com sucesso</response>
    /// <response code="400">Requisição inválida, usuário não é membro ou não é owner</response>
    /// <response code="401">Usuário não autorizado(Token inválido ou vencido)</response>
    /// <response code="404">Grupo ou usuário não encontrado</response>
    [HttpPut("{groupId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateGroupById([FromRoute] Guid groupId,
        [FromBody] UpdateGroupRequest request)
    {
        var currentUserId = currentUser.GetId();

        if (!await userRepository.ExistsByIdAsync(currentUserId))
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var group = await groupRepository.GetGroupWithMembersAsync(groupId);

        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var currentUserMember = group.Members.SingleOrDefault(m => m.UserId == currentUserId);

        if (currentUserMember is null)
        {
            return BadRequest(new ErrorResponse("You are not a member of this group."));
        }

        if (!currentUserMember.IsOwner)
        {
            return BadRequest(new ErrorResponse("Only the group owner can update group data."));
        }

        group.Update(request.Name, request.TripExpectedDate);
        groupRepository.Update(group);
        await unitOfWork.SaveAsync();

        return NoContent();
    }
    /// <summary>
    /// Deleta um grupo pelo Id.
    /// </summary>
    /// <param name="groupId"></param>
    /// <response code="204">Grupo deletado com sucesso</response>
    /// <response code="400">Usuário não é membro ou não é owner</response>
    /// <response code="401">Usuário não autorizado(Token inválido ou vencido)</response>
    /// <response code="404">Grupo ou usuário não encontrado</response>
    [HttpDelete("{groupId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteGroupById([FromRoute] Guid groupId)
    {
        var currentUserId = currentUser.GetId();

        if (!await userRepository.ExistsByIdAsync(currentUserId))
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var group = await groupRepository.GetGroupWithMembersAsync(groupId);

        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var currentUserMember = group.Members.SingleOrDefault(m => m.UserId == currentUserId);

        if (currentUserMember is null)
        {
            return BadRequest(new ErrorResponse("You are not a member of this group."));
        }

        if (!currentUserMember.IsOwner)
        {
            return BadRequest(new ErrorResponse("Only the group owner can delete the group."));
        }

        groupRepository.Remove(group);
        await unitOfWork.SaveAsync();

        return NoContent();
    }
}

