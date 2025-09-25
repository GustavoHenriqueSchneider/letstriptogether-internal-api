using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApi.Context;
using WebApi.DTOs.Requests.Auth;
using WebApi.DTOs.Responses;
using WebApi.DTOs.Responses.Auth;
using WebApi.Models;
using WebApi.Security;
using WebApi.Services;

namespace WebApi.Controllers;

// TODO: aplicar CQRS com usecases, mediator com mediatr, repository, DI e clean arc
// TODO: colocar tag de versionamento e descricoes para swagger
// TODO: definir retorno das rotas com classes de response e converter returns de erro em exception
[ApiController]
[Route("api/v1/auth")]
public class AuthController(AppDbContext context, ITokenService tokenService,
    IPasswordHashService passwordHashService, IApplicationUserContext currentUser) : ControllerBase
{
    [HttpPost("email/send")]
    [AllowAnonymous]
    public async Task<IActionResult> SendRegisterConfirmationEmail(
        [FromBody] SendRegisterConfirmationEmailRequest request)
    {
        var existsUserWithEmail = await context.Users.AnyAsync(x => x.Email == request.Email);

        if (existsUserWithEmail)
        {
            return Conflict(new BaseResponse
            {
                Status = "Error",
                Message = "There is already an user using this email."
            });
        }

        // TODO: criar service para envio de email e outra para armazenar codigo no redis

        var claims = new List<Claim>
        {
            new (Claims.Name, request.Name),
            new (ClaimTypes.Email, request.Email)
        };

        var token = tokenService.GenerateRegisterTokenForStep(Steps.ValidateEmail, claims);
        return Ok(new BaseResponse
        {
            Status = "Success",
            Message = "Confirmation email sent.",
            Data = new SendRegisterConfirmationEmailResponse { Token = token }
        });
    }

    [HttpPost("email/validate")]
    [Authorize(Policy = Policies.RegisterValidateEmail)]
    public async Task<IActionResult> ValidateRegisterConfirmationCode(
        [FromBody] ValidateRegisterConfirmationCodeRequest request)
    {
        // TODO: verificar se codigo armazenado para email atual no redis confere
        if (request.Code != 123456)
        {
            return BadRequest(new BaseResponse { Status = "Error", Message = "Invalid code." });
        }

        var claims = new List<Claim>
        {
            new (Claims.Name, currentUser.GetName()),
            new (ClaimTypes.Email, currentUser.GetEmail())
        };

        var token = tokenService.GenerateRegisterTokenForStep(Steps.SetPassword, claims);
        return Ok(new BaseResponse
        {
            Status = "Success",
            Message = "Email confirmed.",
            Data = new ValidateRegisterConfirmationCodeResponse { Token = token }
        });
    }

    [HttpPost("register")]
    [Authorize(Policy = Policies.RegisterSetPassword)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var email = currentUser.GetEmail();
        var existsUserWithEmail = await context.Users.AnyAsync(x => x.Email == email);

        if (existsUserWithEmail)
        {
            return Conflict(new BaseResponse
            {
                Status = "Error",
                Message = "There is already an user using this email."
            });
        }

        var defaultRole = await context.Roles.SingleOrDefaultAsync(x => x.Name == Roles.User);

        if (defaultRole is null)
        {
            return NotFound(new BaseResponse
            {
                Status = "Error",
                Message = "Role not found."
            });
        }

        var passwordHash = passwordHashService.HashPassword(request.Password);
        var user = new User(currentUser.GetName(), email, passwordHash, defaultRole);

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(Register), new BaseResponse
        {
            Status = "Success",
            Message = "User was registered.",
            Data = new RegisterResponse { Id = user.Id }
        });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await context.Users
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Email == request.Email);

        if (user is null)
        {
            return Unauthorized(new BaseResponse
            {
                Status = "Error",
                Message = "Invalid credentials."
            });
        }

        var isCorrectPassword = passwordHashService.VerifyPassword(request.Password, user.PasswordHash);

        if (!isCorrectPassword)
        {
            return Unauthorized(new BaseResponse
            {
                Status = "Error",
                Message = "Invalid credentials."
            });
        }

        var (accessToken, refreshToken) = tokenService.GenerateTokens(user);

        // TODO: armazenar refreshtoken no redis em auth:refresh_token:{userId}

        return Ok(new BaseResponse
        {
            Status = "Success",
            Message = "User was logged in.",
            Data = new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            }
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userExists = await context.Users.AnyAsync(x => x.Id == currentUser.GetId());

        if (!userExists)
        {
            return NotFound(new BaseResponse
            {
                Status = "Error",
                Message = "User not found."
            });
        }

        // TODO: remover refreshtoken no redis em auth:refresh_token:{userId}

        return NoContent();
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        // TODO: verificar no redis se existe o request.RefreshToken pelo id do usuario do token

        // TODO: se token estiver expirado retornar unauthorized
        // TODO: se token for invalido retornar unauthorized

        // TODO: usar id do usuario com base no refreshtoken no redis
        var id = Guid.Empty;
        var user = await context.Users
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id);

        if (user is null)
        {
            return NotFound(new BaseResponse
            {
                Status = "Error",
                Message = "User not found."
            });
        }

        var (accessToken, refreshToken) = tokenService.GenerateTokens(user);

        // TODO: armazenar refreshtoken no redis em auth:refresh_token:{userId}

        return Ok(new BaseResponse
        {
            Status = "Success",
            Message = "Tokens were refreshed.",
            Data = new RefreshTokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            }
        });
    }

    [HttpPost("reset-password/request")]
    [AllowAnonymous]
    public async Task<IActionResult> RequestResetPassword([FromBody] RequestResetPasswordRequest request) 
    {
        var user = await context.Users.SingleOrDefaultAsync(x => x.Email == request.Email);

        if (user is null)
        {
            return Accepted(new BaseResponse
            {
                Status = "Success",
                Message = "If the informed email exists, an message will be sent."
            });
        }

        var token = tokenService.GenerateResetPasswordToken(user.Id);
        
        // TODO: criar service para envio de email e outra para armazenar token no redis

        return Accepted(new BaseResponse
        {
            Status = "Success",
            Message = "If the informed email exists, an message will be sent.",
            // TODO: remover token do response após criar service de enviar email
            Data = token
        });
    }

    [HttpPost("reset-password")]
    [Authorize(Policy = Policies.ResetPassword)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request) 
    {
        var user = await context.Users.SingleOrDefaultAsync(x => x.Id == currentUser.GetId());

        if (user is null)
        {
            return NotFound(new BaseResponse
            {
                Status = "Error",
                Message = "User not found."
            });
        }

        // TODO: validar se o token de reset é o ultimo armazenado pra esse user no redis

        var passwordHash = passwordHashService.HashPassword(request.Password);

        user.SetPassword(passwordHash);
        await context.SaveChangesAsync();

        // TODO: remover token do redis

        return NoContent();
    }
}
