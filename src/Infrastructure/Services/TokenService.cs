using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Security;
using LetsTripTogether.InternalApi.Domain.ValueObjects;
using LetsTripTogether.InternalApi.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Claim = System.Security.Claims.Claim;
using Claims = LetsTripTogether.InternalApi.Domain.Security.Claim;

namespace LetsTripTogether.InternalApi.Infrastructure.Services;

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
            new(Claims.TokenType, TokenType.Step),
            new(Claims.Step, new Step(step))
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
            new (Claims.TokenType, TokenType.ResetPassword)
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
            new (Claims.TokenType, TokenType.Invitation)
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
            new (Claims.TokenType, TokenType.Access)
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
            new (Claims.TokenType, TokenType.Refresh)
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

            if (tokenType is null || tokenType != TokenType.Refresh)
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

            if (tokenType is null || tokenType != TokenType.Invitation)
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
