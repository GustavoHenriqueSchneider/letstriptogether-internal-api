using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Configurations;
using WebApi.Models.Aggregates;
using WebApi.Security;
using WebApi.Services.Interfaces;

namespace WebApi.Services.Implementations;

public class TokenService : ITokenService
{
    private readonly JsonWebTokenSettings _jwtSettings;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly byte[] _key;

    public TokenService(IOptions<JsonWebTokenSettings> jwtSettings, JwtSecurityTokenHandler tokenHandler)
    {
        _jwtSettings = jwtSettings.Value;
        _tokenHandler = tokenHandler;

        var key = _jwtSettings.SecretKey ?? throw new InvalidOperationException("Invalid secret key");
        _key = Encoding.UTF8.GetBytes(key);
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

    public string GenerateInvitationToken(Guid invitationId)
    {
        var claims = new List<Claim>
        {
            new (Claims.Id, invitationId.ToString()),
            new (Claims.TokenType, TokenTypes.Invitation)
        };

        var expiresIn = DateTime.UtcNow.AddMinutes(_jwtSettings.InvitationTokenValidityInMinutes);

        var token = CreateJwtToken(claims, expiresIn);
        return token;
    }

    public (string accessToken, string refreshToken) GenerateTokens(User user, DateTime? refreshTokenExpiresIn = null)
    {
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken(user.Id, refreshTokenExpiresIn);

        return (accessToken, refreshToken);
    }

    private string CreateJwtToken(List<Claim> claims, DateTime expiresIn)
    {
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(_key),
            SecurityAlgorithms.HmacSha256Signature
        );

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            SigningCredentials = signingCredentials,
            Subject = new ClaimsIdentity(claims),
            Issuer = _jwtSettings.Issuer,
            Expires = expiresIn
        };

        return _tokenHandler.CreateJwtSecurityToken(tokenDescriptor).RawData;
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

    private string GenerateRefreshToken(Guid userId, DateTime? expiresIn)
    {
        var claims = new List<Claim>
        {
            new (Claims.Id, userId.ToString()),
            new (Claims.TokenType, TokenTypes.Refresh)
        };

        expiresIn ??= DateTime.UtcNow.AddMinutes(_jwtSettings.RefreshTokenValidityInMinutes);

        var refreshToken = CreateJwtToken(claims, expiresIn.Value);
        return refreshToken;
    }

    public (bool isValid, ClaimsPrincipal? claims) ValidateRefreshToken(string refreshToken)
    {
        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = _jwtSettings.Issuer,
                IssuerSigningKey = new SymmetricSecurityKey(_key)
            };

            var claims = _tokenHandler.ValidateToken(refreshToken, validationParameters, out _);
            var tokenType = claims.FindFirstValue(Claims.TokenType);

            if (tokenType is null || tokenType != TokenTypes.Refresh)
            {
                return (false, null);
            }

            return (true, claims);
        }
        catch
        {
            return (false, null);
        }
    }
    
    public (bool isValid, string? invitationId) ValidateInvitationToken(string invitationToken)
    {
        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = _jwtSettings.Issuer,
                IssuerSigningKey = new SymmetricSecurityKey(_key)
            };

            var claims = _tokenHandler.ValidateToken(invitationToken, validationParameters, out _);
            var tokenType = claims.FindFirstValue(Claims.TokenType);

            if (tokenType is null || tokenType != TokenTypes.Invitation)
            {
                return (false, null);
            }
            
            var invitationId = claims.FindFirstValue(Claims.Id);
            return (true, invitationId);
        }
        catch
        {
            return (false, null);
        }
    }

    public (bool isExpired, DateTime? expiresIn) IsTokenExpired(string token)
    {
        try
        {
            var jwtToken = _tokenHandler.ReadJwtToken(token);
            var expiresIn = jwtToken.ValidTo;
            var isExpired = expiresIn < DateTime.UtcNow;

            return (isExpired, expiresIn);
        }
        catch
        {
            return (true, null);
        }
    }
}
