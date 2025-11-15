using Application.Common.Interfaces.Extensions;
using Application.Common.Policies;
using Application.UseCases.v1.Auth.Command.Login;
using Application.UseCases.v1.Auth.Command.Logout;
using Application.UseCases.v1.Auth.Command.RefreshToken;
using Application.UseCases.v1.Auth.Command.Register;
using Application.UseCases.v1.Auth.Command.RequestResetPassword;
using Application.UseCases.v1.Auth.Command.ResetPassword;
using Application.UseCases.v1.Auth.Command.SendRegisterConfirmationEmail;
using Application.UseCases.v1.Auth.Command.ValidateRegisterConfirmationCode;
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
    public async Task<IActionResult> SendRegisterConfirmationEmail(
        [FromBody] SendRegisterConfirmationEmailCommand command, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpPost("email/validate")]
    [Authorize(Policy = Policies.RegisterValidateEmail)]
    [SwaggerOperation(
        Summary = "Validar Código de Confirmação de Email",
        Description = "Valida o código de confirmação enviado por email durante o processo de registro. Requer token de validação de email.")]
    [ProducesResponseType(typeof(ValidateRegisterConfirmationCodeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [SwaggerOperation(
        Summary = "Fazer Login",
        Description = "Autentica um usuário com email e senha, retornando tokens de acesso e refresh.")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [SwaggerOperation(
        Summary = "Atualizar Token",
        Description = "Gera novos tokens de acesso e refresh usando um token de refresh válido.")]
    [ProducesResponseType(typeof(RefreshTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpPost("reset-password/request")]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Solicitar Redefinição de Senha",
        Description = "Envia um email com link para redefinição de senha. O link é válido por um período limitado.")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
