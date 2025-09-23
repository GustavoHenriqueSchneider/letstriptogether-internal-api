using Microsoft.AspNetCore.Authentication.BearerToken;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebApi.Models;

namespace WebApi.Services;

public interface IAuthService
{
    string GenerateRegisterTokenForStep(string step, List<Claim> claims);
    (string accessToken, string refreshToken) GenerateTokens(User user);
}
