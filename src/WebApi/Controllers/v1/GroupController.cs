using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Extensions;
using LetsTripTogether.InternalApi.Application.UseCases.Group.Command.CreateGroup;
using LetsTripTogether.InternalApi.Application.UseCases.Group.Command.DeleteGroupById;
using LetsTripTogether.InternalApi.Application.UseCases.Group.Command.LeaveGroupById;
using LetsTripTogether.InternalApi.Application.UseCases.Group.Command.UpdateGroupById;
using LetsTripTogether.InternalApi.Application.UseCases.Group.Query.GetAllGroups;
using LetsTripTogether.InternalApi.Application.UseCases.Group.Query.GetGroupById;
using LetsTripTogether.InternalApi.Application.UseCases.Group.Query.GetNotVotedDestinationsByMemberOnGroup;

namespace LetsTripTogether.InternalApi.WebApi.Controllers.v1;

// TODO: colocar tag de versionamento e descricoes para swagger

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/groups")]
public class GroupController(
    IMediator mediator,
    IApplicationUserContextExtensions currentUser) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupCommand command, CancellationToken cancellationToken)
    {
        command = command with
        {
            UserId = currentUser.GetId()
        };

        var response = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(CreateGroup), response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllGroups([FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new GetAllGroupsQuery
        {
            UserId = currentUser.GetId(),
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{groupId:guid}")]
    public async Task<IActionResult> GetGroupById([FromRoute] Guid groupId, CancellationToken cancellationToken)
    {
        var query = new GetGroupByIdQuery
        {
            GroupId = groupId,
            UserId = currentUser.GetId()
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{groupId:guid}")]
    public async Task<IActionResult> UpdateGroupById([FromRoute] Guid groupId, 
        [FromBody] UpdateGroupByIdCommand command, CancellationToken cancellationToken)
    {
        command = command with
        {
            GroupId = groupId,
            UserId = currentUser.GetId()
        };

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{groupId:guid}")]
    public async Task<IActionResult> DeleteGroupById([FromRoute] Guid groupId, CancellationToken cancellationToken)
    {
        var command = new DeleteGroupByIdCommand
        {
            GroupId = groupId,
            UserId = currentUser.GetId()
        };

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
    
    [HttpPatch("{groupId:guid}/leave")]
    public async Task<IActionResult> LeaveGroupById([FromRoute] Guid groupId, CancellationToken cancellationToken)
    {
        var command = new LeaveGroupByIdCommand
        {
            GroupId = groupId,
            UserId = currentUser.GetId()
        };

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
    
    [HttpGet("{groupId:guid}/destinations-not-voted")]
    public async Task<IActionResult> GetNotVotedDestinationsByMemberOnGroup([FromRoute] Guid groupId,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new GetNotVotedDestinationsByMemberOnGroupQuery
        {
            GroupId = groupId,
            UserId = currentUser.GetId(),
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }
}
