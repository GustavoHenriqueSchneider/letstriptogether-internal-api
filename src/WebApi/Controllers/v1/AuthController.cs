using System.Security.Claims;
using LetsTripTogether.InternalApi.Application.Common.Extensions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Application.Common.Policies;
using LetsTripTogether.InternalApi.Application.Helpers;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Common;
using LetsTripTogether.InternalApi.Domain.Security;
using LetsTripTogether.InternalApi.Domain.ValueObjects;
using LetsTripTogether.InternalApi.Infrastructure.DTOs.Requests.Auth;
using LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses;
using LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetsTripTogether.InternalApi.WebApi.Controllers.v1;

// TODO: aplicar CQRS com usecases, mediator com mediatr e clean arc
// TODO: colocar tag de versionamento e descricoes para swagger
// TODO: converter returns de erro em exception

[ApiController]
[Route("api/v{version:apiVersion}/auth")]
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
    [HttpPost("email/send")]
    [AllowAnonymous]
    public async Task<IActionResult> SendRegisterConfirmationEmail(
        [FromBody] SendRegisterConfirmationEmailRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email;
        var existsUserWithEmail = await userRepository.ExistsByEmailAsync(email, cancellationToken);

        if (existsUserWithEmail)
        {
            return Conflict(new ErrorResponse("There is already an user using this email."));
        }

        var claims = new List<Claim>
        {
            new (Claims.Name, request.Name),
            new (ClaimTypes.Email, email)
        };

        var token = tokenService.GenerateRegisterTokenForStep(Step.ValidateEmail, claims);
        var (_, expiresIn) = tokenService.IsTokenExpired(token);

        var key = KeyHelper.RegisterEmailConfirmation(email);
        var ttlInSeconds = (int)(expiresIn! - DateTime.UtcNow).Value.TotalSeconds;

        var code = randomCodeGeneratorService.Generate();
        await redisService.SetAsync(key, code, ttlInSeconds);
        // TODO: tirar valor hard coded e criar templates de email
        await emailSenderService.SendAsync(request.Email, "Email Confirmation", code, cancellationToken);

        return Ok(new SendRegisterConfirmationEmailResponse { Token = token });
    }

    [HttpPost("email/validate")]
    [Authorize(Policy = Policies.RegisterValidateEmail)]
    public async Task<IActionResult> ValidateRegisterConfirmationCode(
        [FromBody] ValidateRegisterConfirmationCodeRequest request, CancellationToken cancellationToken)
    {
        var key = KeyHelper.RegisterEmailConfirmation(currentUser.GetEmail());
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

        var token = tokenService.GenerateRegisterTokenForStep(Step.SetPassword, claims);
        await redisService.DeleteAsync(key);

        return Ok(new ValidateRegisterConfirmationCodeResponse { Token = token });
    }

    [HttpPost("register")]
    [Authorize(Policy = Policies.RegisterSetPassword)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        if (!request.HasAcceptedTermsOfUse)
        {
            return BadRequest(new ErrorResponse("Terms of use must be accepted for user registration."));
        }

        var email = currentUser.GetEmail();
        var existsUserWithEmail = await userRepository.ExistsByEmailAsync(email, cancellationToken);

        if (existsUserWithEmail)
        {
            return Conflict(new ErrorResponse("There is already an user using this email."));
        }

        var defaultRole = await roleRepository.GetDefaultUserRoleAsync(cancellationToken);

        if (defaultRole is null)
        {
            return NotFound(new ErrorResponse("Default role not found."));
        }

        var passwordHash = passwordHashService.HashPassword(request.Password);
        var user = new User(currentUser.GetName(), email, passwordHash, defaultRole);

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveAsync(cancellationToken);

        return CreatedAtAction(nameof(Register), new RegisterResponse { Id = user.Id });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserWithRolesByEmailAsync(request.Email, cancellationToken);

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

        var key = KeyHelper.UserRefreshToken(user.Id);
        var ttlInSeconds = (int)(expiresIn! - DateTime.UtcNow).Value.TotalSeconds;

        await redisService.SetAsync(key, refreshToken, ttlInSeconds);

        return Ok(new LoginResponse{ AccessToken = accessToken, RefreshToken = refreshToken });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var userId = currentUser.GetId();
        var userExists = await userRepository.ExistsByIdAsync(userId, cancellationToken);

        if (!userExists)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var key = KeyHelper.UserRefreshToken(userId);
        await redisService.DeleteAsync(key);

        // TODO: fazer logica pra travar accesstoken usado no logout enquanto ainda nao expirar

        return NoContent();
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
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

        var key = KeyHelper.UserRefreshToken(userId);
        var storedRefreshToken = await redisService.GetAsync(key);

        if (storedRefreshToken is null || storedRefreshToken != request.RefreshToken)
        {
            return Unauthorized(new ErrorResponse("Invalid refresh token."));
        }

        var user = await userRepository.GetUserWithRolesByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var (accessToken, refreshToken) = tokenService.GenerateTokens(user, refreshTokenExpiresIn);
        var ttlInSeconds = (int)(refreshTokenExpiresIn! - DateTime.UtcNow).Value.TotalSeconds;

        await redisService.SetAsync(key, refreshToken, ttlInSeconds);

        return Ok(new RefreshTokenResponse { AccessToken = accessToken, RefreshToken = refreshToken });
    }

    [HttpPost("reset-password/request")]
    [AllowAnonymous]
    public async Task<IActionResult> RequestResetPassword([FromBody] RequestResetPasswordRequest request, CancellationToken cancellationToken) 
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            return Accepted(new BaseResponse("If the informed email exists, an message will be sent."));
        }

        var resetPasswordToken = tokenService.GenerateResetPasswordToken(user.Id);
        var (_, expiresIn) = tokenService.IsTokenExpired(resetPasswordToken);

        var key = KeyHelper.UserResetPassword(user.Id);
        var ttlInSeconds = (int)(expiresIn! - DateTime.UtcNow).Value.TotalSeconds;

        await redisService.SetAsync(key, resetPasswordToken, ttlInSeconds);
        // TODO: tirar valor hard coded e criar templates de email
        await emailSenderService.SendAsync(request.Email, "Reset Password", resetPasswordToken, cancellationToken);

        return Accepted(new BaseResponse("If the informed email exists, an message will be sent."));
    }

    [HttpPost("reset-password")]
    [Authorize(Policy = Policies.ResetPassword)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken) 
    {
        var user = await userRepository.GetByIdAsync(currentUser.GetId(), cancellationToken);

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var key = KeyHelper.UserResetPassword(user.Id);
        var storedResetPasswordToken = await redisService.GetAsync(key);

        if (storedResetPasswordToken is null || storedResetPasswordToken != HttpContext.GetBearerToken())
        {
            return Unauthorized(new ErrorResponse("Invalid reset password token."));
        }

        var passwordHash = passwordHashService.HashPassword(request.Password);

        user.SetPasswordHash(passwordHash);
        userRepository.Update(user);

        await unitOfWork.SaveAsync(cancellationToken);
        await redisService.DeleteAsync(key);

        return NoContent();
    }
}
