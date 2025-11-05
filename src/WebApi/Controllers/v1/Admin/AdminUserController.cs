using LetsTripTogether.InternalApi.Application.Common.Policies;
using LetsTripTogether.InternalApi.Application.UseCases.AdminUser.Command.AdminCreateUser;
using LetsTripTogether.InternalApi.Application.UseCases.AdminUser.Command.AdminUpdateUserById;
using LetsTripTogether.InternalApi.Application.UseCases.AdminUser.Command.AdminDeleteUserById;
using LetsTripTogether.InternalApi.Application.UseCases.AdminUser.Command.AdminAnonymizeUserById;
using LetsTripTogether.InternalApi.Application.UseCases.AdminUser.Command.AdminSetUserPreferencesByUserId;
using LetsTripTogether.InternalApi.Application.UseCases.AdminUser.Query.AdminGetAllUsers;
using LetsTripTogether.InternalApi.Application.UseCases.AdminUser.Query.AdminGetUserById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetsTripTogether.InternalApi.WebApi.Controllers.v1.Admin;

// TODO: colocar tag de versionamento e descricoes para swagger

[ApiController]
[Authorize(Policy = Policies.Admin)]
[Route("api/v{version:apiVersion}/admin/users")]
public class AdminUserController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> AdminGetAllUsers([FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new AdminGetAllUsersQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> AdminGetUserById([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var query = new AdminGetUserByIdQuery
        {
            UserId = userId
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> AdminCreateUser([FromBody] AdminCreateUserCommand command, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(AdminCreateUser), response);
    }

    [HttpPut("{userId:guid}")]
    public async Task<IActionResult> AdminUpdateUserById([FromRoute] Guid userId, 
        [FromBody] AdminUpdateUserByIdCommand command, CancellationToken cancellationToken)
    {
        var commandWithContext = new AdminUpdateUserByIdCommand
        {
            UserId = userId,
            Name = command.Name
        };

        await mediator.Send(commandWithContext, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> AdminDeleteUserById([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var command = new AdminDeleteUserByIdCommand
        {
            UserId = userId
        };

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{userId:guid}/anonymize")]
    public async Task<IActionResult> AdminAnonymizeUserById([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var command = new AdminAnonymizeUserByIdCommand
        {
            UserId = userId
        };

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPut("{userId:guid}/preferences")]
    public async Task<IActionResult> AdminSetUserPreferencesByUserId([FromRoute] Guid userId, 
        [FromBody] AdminSetUserPreferencesByUserIdCommand command, CancellationToken cancellationToken)
    {
        var commandWithContext = new AdminSetUserPreferencesByUserIdCommand
        {
            UserId = userId,
            LikesCommercial = command.LikesCommercial,
            Food = command.Food,
            Culture = command.Culture,
            Entertainment = command.Entertainment,
            PlaceTypes = command.PlaceTypes
        };

        await mediator.Send(commandWithContext, cancellationToken);
        return NoContent();
    }
}
