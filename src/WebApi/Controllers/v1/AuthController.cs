using Application.Common.Interfaces.Extensions;
using Application.Common.Policies;
using Application.UseCases.Auth.Command.Login;
using Application.UseCases.Auth.Command.Logout;
using Application.UseCases.Auth.Command.RefreshToken;
using Application.UseCases.Auth.Command.Register;
using Application.UseCases.Auth.Command.RequestResetPassword;
using Application.UseCases.Auth.Command.ResetPassword;
using Application.UseCases.Auth.Command.SendRegisterConfirmationEmail;
using Application.UseCases.Auth.Command.ValidateRegisterConfirmationCode;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers.v1;

[ApiController]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController(
    IMediator mediator,
    IApplicationUserContextExtensions currentUser,
    IHttpContextExtensions httpContextExtensions) : ControllerBase
{
    [HttpPost("email/send")]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Enviar Email de Confirmação de Registro",
        Description = "Envia um código de confirmação por email para validar o endereço de email durante o processo de registro.")]
    [ProducesResponseType(typeof(SendRegisterConfirmationEmailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
    [SwaggerOperation(
        Summary = "Registrar Novo Usuário",
        Description = "Cria uma nova conta de usuário após validação do email e definição da senha. Requer aceitação dos termos de uso.")]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
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
    [SwaggerOperation(
        Summary = "Fazer Logout",
        Description = "Invalida o token de refresh do usuário autenticado, efetuando logout.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [SwaggerOperation(
        Summary = "Redefinir Senha",
        Description = "Redefine a senha do usuário usando o token de redefinição de senha válido.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
