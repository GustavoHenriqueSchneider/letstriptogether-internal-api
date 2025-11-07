using LetsTripTogether.InternalApi.Application.Common.Interfaces.Extensions;
using LetsTripTogether.InternalApi.Application.UseCases.User.Command.AnonymizeCurrentUser;
using LetsTripTogether.InternalApi.Application.UseCases.User.Command.DeleteCurrentUser;
using LetsTripTogether.InternalApi.Application.UseCases.User.Command.SetCurrentUserPreferences;
using LetsTripTogether.InternalApi.Application.UseCases.User.Command.UpdateCurrentUser;
using LetsTripTogether.InternalApi.Application.UseCases.User.Query.GetCurrentUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetsTripTogether.InternalApi.WebApi.Controllers.v1;

// TODO: descricoes para swagger

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/users/me")]
public class UserController(
    IMediator mediator,
    IApplicationUserContextExtensions currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var query = new GetCurrentUserQuery
        {
            UserId = currentUser.GetId()
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCurrentUser(
        [FromBody] UpdateCurrentUserCommand command, CancellationToken cancellationToken)
    {
        command = command with
        {
            UserId = currentUser.GetId()
        };

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteCurrentUser(CancellationToken cancellationToken)
    {
        var command = new DeleteCurrentUserCommand
        {
            UserId = currentUser.GetId()
        };

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPatch("anonymize")]
    public async Task<IActionResult> AnonymizeCurrentUser(CancellationToken cancellationToken)
    {
        var command = new AnonymizeCurrentUserCommand
        {
            UserId = currentUser.GetId()
        };

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPut("preferences")]
    public async Task<IActionResult> SetCurrentUserPreferences(
        [FromBody] SetCurrentUserPreferencesCommand command, CancellationToken cancellationToken)
    {
        command = command with
        {
            UserId = currentUser.GetId()
        };

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
