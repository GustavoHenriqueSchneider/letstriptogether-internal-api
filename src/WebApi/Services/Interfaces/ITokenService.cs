using Microsoft.AspNetCore.Authentication.BearerToken;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebApi.Models;

namespace WebApi.Services.Interfaces;

public interface ITokenService
{
    string GenerateRegisterTokenForStep(string step, List<Claim> claims);
    string GenerateResetPasswordToken(Guid userId);
    (string accessToken, string refreshToken) GenerateTokens(User user);
}
