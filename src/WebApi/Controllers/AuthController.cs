using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApi.Context.Interfaces;
using WebApi.DTOs.Requests.Auth;
using WebApi.DTOs.Responses;
using WebApi.DTOs.Responses.Auth;
using WebApi.Extensions;
using WebApi.Models;
using WebApi.Persistence.Interfaces;
using WebApi.Repositories.Interfaces;
using WebApi.Security;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers;

// TODO: aplicar CQRS com usecases, mediator com mediatr e clean arc
// TODO: colocar tag de versionamento e descricoes para swagger
// TODO: converter returns de erro em exception

[ApiController]
[Route("api/v1/auth")]
[Tags("Autenticação")]
public class AuthController(
    IEmailSenderService emailSenderService, 
    IPasswordHashService passwordHashService, 
    IRandomCodeGeneratorService randomCodeGeneratorService,
    IRedisService redisService,
    ITokenService tokenService,
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork,
    IApplicationUserContext currentUser) : ControllerBase
{
    /// <summary>
    /// Envia email de confirmação para registro.
    /// </summary>
    /// <param name="request"></param>
    /// <response code="200">Email enviado com sucesso</response>
    /// <response code="409">Já existe um usuário usando este email</response>
    /// <returns></returns>
    [HttpPost("email/send")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SendRegisterConfirmationEmailResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SendRegisterConfirmationEmail(
        [FromBody] SendRegisterConfirmationEmailRequest request)
    {
        var email = request.Email;
        var existsUserWithEmail = await userRepository.ExistsByEmailAsync(email);

        if (existsUserWithEmail)
        {
            return Conflict(new ErrorResponse("There is already an user using this email."));
        }

        var claims = new List<Claim>
        {
            new (Claims.Name, request.Name),
            new (ClaimTypes.Email, email)
        };

        var token = tokenService.GenerateRegisterTokenForStep(Steps.ValidateEmail, claims);
        var (_, expiresIn) = tokenService.IsTokenExpired(token);

        var key = RedisKeys.RegisterEmailConfirmation.Replace("{email}", email);
        var ttlInSeconds = (int)(expiresIn! - DateTime.UtcNow).Value.TotalSeconds;

        var code = randomCodeGeneratorService.Generate();
        await redisService.SetAsync(key, code, ttlInSeconds);
        // TODO: tirar valor hard coded e criar templates de email
        await emailSenderService.SendAsync(request.Email, "Email Confirmation", code);

        return Ok(new SendRegisterConfirmationEmailResponse { Token = token });
    }
    /// <summary>
    /// Valida código de confirmação de email.
    /// </summary>
    /// <param name="request"></param>
    /// <response code="200">Código validado com sucesso</response>
    /// <response code="400">Código inválido</response>
    /// <response code="401">Usuário não autorizado(Token inválido ou vencido)</response>
    [HttpPost("email/validate")]
    [Authorize(Policy = Policies.RegisterValidateEmail)]
    [ProducesResponseType(typeof(ValidateRegisterConfirmationCodeResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ValidateRegisterConfirmationCode(
        [FromBody] ValidateRegisterConfirmationCodeRequest request)
    {
        var key = RedisKeys.RegisterEmailConfirmation.Replace("{email}", currentUser.GetEmail());
        var code = await redisService.GetAsync(key);

        if (code is null || code != request.Code)
        {
            return BadRequest(new ErrorResponse("Invalid code."));
        }

        var claims = new List<Claim>
        {
            new (Claims.Name, currentUser.GetName()),
            new (ClaimTypes.Email, currentUser.GetEmail())
        };

        var token = tokenService.GenerateRegisterTokenForStep(Steps.SetPassword, claims);
        await redisService.DeleteAsync(key);

        return Ok(new ValidateRegisterConfirmationCodeResponse { Token = token });
    }
    /// <summary>
    /// Registra um novo usuário.
    /// </summary>
    /// <param name="request"></param>
    /// <response code="201">Usuário registrado com sucesso</response>
    /// <response code="400">Requisição inválida</response>
    /// <response code="401">Usuário não autorizado(Token inválido ou vencido)</response>
    /// <response code="404">Role padrão não encontrado</response>
    /// <response code="409">Já existe um usuário usando este email</response>
    [HttpPost("register")]
    [Authorize(Policy = Policies.RegisterSetPassword)]
    [ProducesResponseType(typeof(RegisterResponse),StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!request.HasAcceptedTermsOfUse)
        {
            return BadRequest(new ErrorResponse("Terms of use must be accepted for user registration."));
        }

        var email = currentUser.GetEmail();
        var existsUserWithEmail = await userRepository.ExistsByEmailAsync(email);

        if (existsUserWithEmail)
        {
            return Conflict(new ErrorResponse("There is already an user using this email."));
        }

        var defaultRole = await roleRepository.GetDefaultUserRoleAsync();

        if (defaultRole is null)
        {
            return NotFound(new ErrorResponse("Default role not found."));
        }

        var passwordHash = passwordHashService.HashPassword(request.Password);
        var user = new User(currentUser.GetName(), email, passwordHash, defaultRole);

        await userRepository.AddAsync(user);
        await unitOfWork.SaveAsync();

        return CreatedAtAction(nameof(Register), new RegisterResponse { Id = user.Id });
    }
    /// <summary>
    /// Realiza login do usuário.
    /// </summary>
    /// <param name="request"></param>
    /// <response code="200">Login realizado com sucesso</response>
    /// <response code="401">Credenciais inválidas</response>
    /// <returns></returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await userRepository.GetUserWithRolesByEmailAsync(request.Email);

        if (user is null)
        {
            return Unauthorized(new ErrorResponse("Invalid credentials."));
        }

        var isCorrectPassword = passwordHashService.VerifyPassword(request.Password, user.PasswordHash);

        if (!isCorrectPassword)
        {
            return Unauthorized(new ErrorResponse("Invalid credentials."));
        }

        var (accessToken, refreshToken) = tokenService.GenerateTokens(user);
        var (_, expiresIn) = tokenService.IsTokenExpired(refreshToken);

        var key = RedisKeys.UserRefreshToken.Replace("{userId}", user.Id.ToString());
        var ttlInSeconds = (int)(expiresIn! - DateTime.UtcNow).Value.TotalSeconds;

        await redisService.SetAsync(key, refreshToken, ttlInSeconds);

        return Ok(new LoginResponse{ AccessToken = accessToken, RefreshToken = refreshToken });
    }
    /// <summary>
    /// Realiza logout do usuário.
    /// </summary>
    /// <response code="204">Logout realizado com sucesso</response>
    /// <response code="401">Usuário não autorizado(Token inválido ou vencido)</response>
    /// <response code="404">Usuário não encontrado</response>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Logout()
    {
        var userId = currentUser.GetId();
        var userExists = await userRepository.ExistsByIdAsync(userId);

        if (!userExists)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var key = RedisKeys.UserRefreshToken.Replace("{userId}", userId.ToString());
        await redisService.DeleteAsync(key);

        // TODO: fazer logica pra travar accesstoken usado no logout enquanto ainda nao expirar

        return NoContent();
    }
    /// <summary>
    /// Atualiza o token de acesso usando o refresh token.
    /// </summary>
    /// <param name="request"></param>
    /// <response code="200">Token atualizado com sucesso</response>
    /// <response code="401">Refresh token inválido ou expirado</response>
    /// <response code="404">Usuário não encontrado</response>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RefreshTokenResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var (isValid, claims) = tokenService.ValidateRefreshToken(request.RefreshToken);

        if (!isValid)
        {
            return Unauthorized(new ErrorResponse("Invalid refresh token."));
        }

        var (isExpired, refreshTokenExpiresIn) = tokenService.IsTokenExpired(request.RefreshToken);

        if (isExpired)
        {
            return Unauthorized(new ErrorResponse("Refresh token has expired."));
        }

        var id = claims!.FindFirstValue(Claims.Id);

        if (!Guid.TryParse(id, out var userId) || userId == Guid.Empty)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var key = RedisKeys.UserRefreshToken.Replace("{userId}", userId.ToString());
        var storedRefreshToken = await redisService.GetAsync(key);

        if (storedRefreshToken is null || storedRefreshToken != request.RefreshToken)
        {
            return Unauthorized(new ErrorResponse("Invalid refresh token."));
        }

        var user = await userRepository.GetUserWithRolesByIdAsync(userId);

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var (accessToken, refreshToken) = tokenService.GenerateTokens(user, refreshTokenExpiresIn);
        var ttlInSeconds = (int)(refreshTokenExpiresIn! - DateTime.UtcNow).Value.TotalSeconds;

        await redisService.SetAsync(key, refreshToken, ttlInSeconds);

        return Ok(new RefreshTokenResponse { AccessToken = accessToken, RefreshToken = refreshToken });
    }
    /// <summary>
    /// Solicita reset de senha.
    /// </summary>
    /// <param name="request"></param>
    /// <response code="202">Se o email informado existe, uma mensagem será enviada</response>
    /// <returns></returns>
    [HttpPost("reset-password/request")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(BaseResponse),StatusCodes.Status202Accepted)]
    public async Task<IActionResult> RequestResetPassword([FromBody] RequestResetPasswordRequest request) 
    {
        var user = await userRepository.GetByEmailAsync(request.Email);

        if (user is null)
        {
            return Accepted(new BaseResponse("If the informed email exists, an message will be sent."));
        }

        var resetPasswordToken = tokenService.GenerateResetPasswordToken(user.Id);
        var (_, expiresIn) = tokenService.IsTokenExpired(resetPasswordToken);

        var key = RedisKeys.UserResetPassword.Replace("{userId}", user.Id.ToString());
        var ttlInSeconds = (int)(expiresIn! - DateTime.UtcNow).Value.TotalSeconds;

        await redisService.SetAsync(key, resetPasswordToken, ttlInSeconds);
        // TODO: tirar valor hard coded e criar templates de email
        await emailSenderService.SendAsync(request.Email, "Reset Password", resetPasswordToken);

        return Accepted(new BaseResponse("If the informed email exists, an message will be sent."));
    }
    /// <summary>
    /// Reseta a senha do usuário.
    /// </summary>
    /// <param name="request"></param>
    /// <response code="204">Senha resetada com sucesso</response>
    /// <response code="400">Requisição inválida</response>
    /// <response code="401">Token de reset inválido ou usuário não autorizado</response>
    /// <response code="404">Usuário não encontrado</response>
    [HttpPost("reset-password")]
    [Authorize(Policy = Policies.ResetPassword)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request) 
    {
        var user = await userRepository.GetByIdAsync(currentUser.GetId());

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var key = RedisKeys.UserResetPassword.Replace("{userId}", user.Id.ToString());
        var storedResetPasswordToken = await redisService.GetAsync(key);

        if (storedResetPasswordToken is null || storedResetPasswordToken != HttpContext.GetBearerToken())
        {
            return Unauthorized(new ErrorResponse("Invalid reset password token."));
        }

        var passwordHash = passwordHashService.HashPassword(request.Password);

        user.SetPasswordHash(passwordHash);
        userRepository.Update(user);

        await unitOfWork.SaveAsync();
        await redisService.DeleteAsync(key);

        return NoContent();
    }
}
