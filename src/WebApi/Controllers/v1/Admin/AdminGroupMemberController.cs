using LetsTripTogether.InternalApi.Application.Common.Policies;
using LetsTripTogether.InternalApi.Application.UseCases.AdminGroupMember.Command.AdminRemoveGroupMemberById;
using LetsTripTogether.InternalApi.Application.UseCases.AdminGroupMember.Query.AdminGetAllGroupMembersById;
using LetsTripTogether.InternalApi.Application.UseCases.AdminGroupMember.Query.AdminGetGroupMemberById;
using LetsTripTogether.InternalApi.Application.UseCases.AdminGroupMember.Query.AdminGetGroupMemberAllDestinationVotesById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetsTripTogether.InternalApi.WebApi.Controllers.v1.Admin;

// TODO: colocar tag de versionamento e descricoes para swagger

[ApiController]
[Authorize(Policy = Policies.Admin)]
[Route("api/v{version:apiVersion}/admin/groups/{groupId:guid}/members")]
public class AdminGroupMemberController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> AdminGetAllGroupMembersById([FromRoute] Guid groupId,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new AdminGetAllGroupMembersByIdQuery
        {
            GroupId = groupId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{memberId:guid}")]
    public async Task<IActionResult> AdminGetGroupMemberById([FromRoute] Guid groupId, 
        [FromRoute] Guid memberId, CancellationToken cancellationToken)
    {
        var query = new AdminGetGroupMemberByIdQuery
        {
            GroupId = groupId,
            MemberId = memberId
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{memberId:guid}")]
    public async Task<IActionResult> AdminRemoveGroupMemberById([FromRoute] Guid groupId,
        [FromRoute] Guid memberId, CancellationToken cancellationToken)
    {
        var command = new AdminRemoveGroupMemberByIdCommand
        {
            GroupId = groupId,
            MemberId = memberId
        };

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpGet("{memberId:guid}/destination-votes")]
    public async Task<IActionResult> AdminGetGroupMemberAllDestinationVotesById([FromRoute] Guid groupId,
        [FromRoute] Guid memberId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new AdminGetGroupMemberAllDestinationVotesByIdQuery
        {
            GroupId = groupId,
            MemberId = memberId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }
}
