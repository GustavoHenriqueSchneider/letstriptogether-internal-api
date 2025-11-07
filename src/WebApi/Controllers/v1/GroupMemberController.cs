using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Extensions;
using LetsTripTogether.InternalApi.Application.UseCases.GroupMember.Command.RemoveGroupMemberById;
using LetsTripTogether.InternalApi.Application.UseCases.GroupMember.Query.GetGroupMemberById;
using LetsTripTogether.InternalApi.Application.UseCases.GroupMember.Query.GetOtherGroupMembersById;

namespace LetsTripTogether.InternalApi.WebApi.Controllers.v1;

// TODO: descricoes para swagger

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/groups/{groupId:guid}/members")]
public class GroupMemberController(
    IMediator mediator,
    IApplicationUserContextExtensions currentUser) : ControllerBase
{
    [HttpGet]
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
    public async Task<IActionResult> RemoveGroupMemberById([FromRoute] Guid groupId,
        [FromRoute] Guid memberId, CancellationToken cancellationToken)
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
