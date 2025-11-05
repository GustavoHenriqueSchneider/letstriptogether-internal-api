using LetsTripTogether.InternalApi.Application.Helpers;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Security;
using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;

namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.RefreshToken;

public class RefreshTokenHandler(
    IRedisService redisService,
    ITokenService tokenService,
    IUserRepository userRepository)
    : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var (isValid, claims) = tokenService.ValidateRefreshToken(request.RefreshToken);

        if (!isValid)
        {
            throw new UnauthorizedException("Invalid refresh token.");
        }

        var (isExpired, refreshTokenExpiresIn) = tokenService.IsTokenExpired(request.RefreshToken);

        if (isExpired)
        {
            throw new UnauthorizedException("Refresh token has expired.");
        }

        var id = claims!.FindFirst(Claims.Id)?.Value;

        if (!Guid.TryParse(id, out var userId) || userId == Guid.Empty)
        {
            throw new NotFoundException("User not found.");
        }

        var key = KeyHelper.UserRefreshToken(userId);
        var storedRefreshToken = await redisService.GetAsync(key);

        if (storedRefreshToken is null || storedRefreshToken != request.RefreshToken)
        {
            throw new UnauthorizedException("Invalid refresh token.");
        }

        var user = await userRepository.GetUserWithRolesByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        var (accessToken, refreshToken) = tokenService.GenerateTokens(user, refreshTokenExpiresIn);
        var ttlInSeconds = (int)(refreshTokenExpiresIn! - DateTime.UtcNow).Value.TotalSeconds;

        await redisService.SetAsync(key, refreshToken, ttlInSeconds);

        return new RefreshTokenResponse { AccessToken = accessToken, RefreshToken = refreshToken };
    }
}
