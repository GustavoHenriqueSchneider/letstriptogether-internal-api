using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using WebApi.Models;
using WebApi.Security;

namespace WebApi.Services;

public class TokenService(IConfiguration configuration) : ITokenService
{
    public string GenerateRegisterTokenForStep(string step, List<Claim> claims)
    {
        var validSteps = new List<string> { Steps.ValidateEmail, Steps.SetPassword };

        if (!validSteps.Contains(step))
        {
            throw new InvalidOperationException("Invalid step");
        }

        claims.AddRange([
            new(Claims.TokenType, TokenTypes.Step),
            new(Claims.Step, step)
        ]);

        var expiresIn = DateTime.UtcNow.AddMinutes(configuration.GetSection("JsonWebTokenSettings")
                .GetValue<int>("AccessTokenValidityInMinutes"));

        var token = CreateJwtToken(claims, expiresIn);
        return token;
    }

    public string GenerateResetPasswordToken(Guid userId)
    {
        var claims = new List<Claim>
        {
            new (Claims.Id, userId.ToString()),
            new (Claims.TokenType, TokenTypes.ResetPassword)
        };

        var expiresIn = DateTime.UtcNow.AddMinutes(configuration.GetSection("JsonWebTokenSettings")
                .GetValue<int>("ResetPasswordTokenValidityInMinutes"));

        var token = CreateJwtToken(claims, expiresIn);
        return token;
    }

    public (string accessToken, string refreshToken) GenerateTokens(User user)
    {
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken(user.Id);

        return (accessToken, refreshToken);
    }

    private string CreateJwtToken(List<Claim> claims, DateTime expiresIn)
    {
        var key = configuration.GetSection("JsonWebTokenSettings").GetValue<string>("SecretKey") ??
                  throw new InvalidOperationException("Invalid secret key");
        var privateKey = Encoding.UTF8.GetBytes(key);

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(privateKey),
            SecurityAlgorithms.HmacSha256Signature
        );

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            SigningCredentials = signingCredentials,
            Subject = new ClaimsIdentity(claims),
            Issuer = configuration.GetSection("JsonWebTokenSettings").GetValue<string>("Issuer"),
            Expires = expiresIn
        };

        return new JwtSecurityTokenHandler().CreateJwtSecurityToken(tokenDescriptor).RawData;
    }

    private string GenerateAccessToken(User user)
    {
        var claims = new List<Claim>
        {
            new (Claims.Id, user.Id.ToString()),
            new (Claims.Name, user.Name),
            new (ClaimTypes.Email, user.Email),
            new (Claims.TokenType, TokenTypes.Access)
        };

        foreach (var userRole in user.UserRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
        }

        var expiresIn = DateTime.UtcNow.AddMinutes(configuration.GetSection("JsonWebTokenSettings")
                        .GetValue<int>("AccessTokenValidityInMinutes"));

        var accessToken = CreateJwtToken(claims, expiresIn);
        return accessToken;
    }

    private string GenerateRefreshToken(Guid userId)
    {
        var claims = new List<Claim>
        {
            new (Claims.Id, userId.ToString()),
            new (Claims.TokenType, TokenTypes.Refresh)
        };

        var expiresIn = DateTime.UtcNow.AddMinutes(configuration.GetSection("JsonWebTokenSettings")
                        .GetValue<int>("RefreshTokenValidityInMinutes"));

        var refreshToken = CreateJwtToken(claims, expiresIn);
        return refreshToken;
    }
}
