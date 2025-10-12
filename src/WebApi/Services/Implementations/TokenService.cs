using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Configurations;
using WebApi.Models;
using WebApi.Security;
using WebApi.Services.Interfaces;

namespace WebApi.Services.Implementations;

public class TokenService : ITokenService
{
    private readonly JsonWebTokenSettings _jwtSettings;

    public TokenService(IOptions<JsonWebTokenSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public string GenerateRegisterTokenForStep(string step, List<Claim> claims)
    {
        claims.AddRange([
            new(Claims.TokenType, TokenTypes.Step),
            new(Claims.Step, new Steps(step))
        ]);

        var expiresIn = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenValidityInMinutes);

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

        var expiresIn = DateTime.UtcNow.AddMinutes(_jwtSettings.ResetPasswordTokenValidityInMinutes);

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
        var key = _jwtSettings.SecretKey ?? throw new InvalidOperationException("Invalid secret key");
        var privateKey = Encoding.UTF8.GetBytes(key);

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(privateKey),
            SecurityAlgorithms.HmacSha256Signature
        );

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            SigningCredentials = signingCredentials,
            Subject = new ClaimsIdentity(claims),
            Issuer = _jwtSettings.Issuer,
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

        var expiresIn = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenValidityInMinutes);

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

        var expiresIn = DateTime.UtcNow.AddMinutes(_jwtSettings.RefreshTokenValidityInMinutes);

        var refreshToken = CreateJwtToken(claims, expiresIn);
        return refreshToken;
    }
    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var key = _jwtSettings.SecretKey ?? throw new InvalidOperationException("Invalid secret key");

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey
                               (Encoding.UTF8.GetBytes(_jwtSettings.SecretKey ?? 
                               throw new InvalidOperationException("Invalid secret key"))),
            ValidateLifetime = false ,
            ClockSkew = TimeSpan.Zero           
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            var tokenType = principal.Claims.FirstOrDefault(c => c.Type == Claims.TokenType)?.Value;
            if (tokenType != TokenTypes.Refresh)
            {
                return null;
            }

            return principal;
        }
        catch (SecurityTokenException)
        {
            return null;
        }
        catch
        {
            return null;
        }
        

    }
}
