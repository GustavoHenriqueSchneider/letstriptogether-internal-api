using LetsTripTogether.InternalApi.Application.Helpers;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;

namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.Login;

public class LoginHandler(
    IPasswordHashService passwordHashService,
    IRedisService redisService,
    ITokenService tokenService,
    IUserRepository userRepository)
    : IRequestHandler<LoginCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserWithRolesByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedException("Invalid credentials.");
        }

        var isCorrectPassword = passwordHashService.VerifyPassword(request.Password, user.PasswordHash);

        if (!isCorrectPassword)
        {
            throw new UnauthorizedException("Invalid credentials.");
        }

        var (accessToken, refreshToken) = tokenService.GenerateTokens(user);
        var (_, expiresIn) = tokenService.IsTokenExpired(refreshToken);

        var key = KeyHelper.UserRefreshToken(user.Id);
        var ttlInSeconds = (int)(expiresIn! - DateTime.UtcNow).Value.TotalSeconds;

        await redisService.SetAsync(key, refreshToken, ttlInSeconds);

        return new LoginResponse { AccessToken = accessToken, RefreshToken = refreshToken };
    }
}
