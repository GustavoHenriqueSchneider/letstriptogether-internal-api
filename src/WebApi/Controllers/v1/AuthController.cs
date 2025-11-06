using LetsTripTogether.InternalApi.Application.Common.Policies;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Extensions;
using LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.Login;
using LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.Logout;
using LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.RefreshToken;
using LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.Register;
using LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.RequestResetPassword;
using LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.ResetPassword;
using LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.SendRegisterConfirmationEmail;
using LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.ValidateRegisterConfirmationCode;

namespace LetsTripTogether.InternalApi.WebApi.Controllers.v1;

// TODO: colocar tag de versionamento e descricoes para swagger

[ApiController]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController(
    IMediator mediator,
    IApplicationUserContextExtensions currentUser,
    IHttpContextExtensions httpContextExtensions) : ControllerBase
{
    [HttpPost("email/send")]
    [AllowAnonymous]
    public async Task<IActionResult> SendRegisterConfirmationEmail(
        [FromBody] SendRegisterConfirmationEmailCommand command, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpPost("email/validate")]
    [Authorize(Policy = Policies.RegisterValidateEmail)]
    public async Task<IActionResult> ValidateRegisterConfirmationCode(
        [FromBody] ValidateRegisterConfirmationCodeCommand command, CancellationToken cancellationToken)
    {
        command = command with
        {
            Email = currentUser.GetEmail(),
            Name = currentUser.GetName()
        };

        var response = await mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpPost("register")]
    [Authorize(Policy = Policies.RegisterSetPassword)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken cancellationToken)
    {
        command = command with
        {
            Email = currentUser.GetEmail(),
            Name = currentUser.GetName()
        };
        
        var response = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(Register), response);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var command = new LogoutCommand
        {
            UserId = currentUser.GetId()
        };

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpPost("reset-password/request")]
    [AllowAnonymous]
    public async Task<IActionResult> RequestResetPassword([FromBody] RequestResetPasswordCommand command, CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);
        return Accepted();
    }

    [HttpPost("reset-password")]
    [Authorize(Policy = Policies.ResetPassword)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        command = command with
        {
            UserId = currentUser.GetId(),
            BearerToken = httpContextExtensions.GetBearerToken() ?? string.Empty
        };

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
