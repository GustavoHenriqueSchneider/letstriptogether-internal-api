using System.Security.Claims;
using WebApi.Models.Aggregates;

namespace WebApi.Services.Interfaces;

public interface ITokenService
{
    string GenerateRegisterTokenForStep(string step, List<Claim> claims);
    string GenerateResetPasswordToken(Guid userId);
    (string accessToken, string refreshToken) GenerateTokens(User user, DateTime? refreshTokenExpiresIn = null);
    (bool isValid, ClaimsPrincipal? claims) ValidateRefreshToken(string refreshToken);
    (bool isValid, string? invitationId) ValidateInvitationToken(string invitationToken);
    (bool isExpired, DateTime? expiresIn) IsTokenExpired(string token);
    string GenerateInvitationToken(Guid invitationId);
}
