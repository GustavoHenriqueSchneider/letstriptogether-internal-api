using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApi.Context.Interfaces;
using WebApi.DTOs.Requests.Auth;
using WebApi.DTOs.Responses;
using WebApi.DTOs.Responses.Auth;
using WebApi.Extensions;
using WebApi.Models.Aggregates;
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

    [HttpPost("email/validate")]
    [Authorize(Policy = Policies.RegisterValidateEmail)]
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

    [HttpPost("register")]
    [Authorize(Policy = Policies.RegisterSetPassword)]
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

    [HttpPost("login")]
    [AllowAnonymous]
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

    [HttpPost("logout")]
    [Authorize]
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

    [HttpPost("refresh")]
    [AllowAnonymous]
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

    [HttpPost("reset-password/request")]
    [AllowAnonymous]
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

    [HttpPost("reset-password")]
    [Authorize(Policy = Policies.ResetPassword)]
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
