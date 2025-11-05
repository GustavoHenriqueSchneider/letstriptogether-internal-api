using LetsTripTogether.InternalApi.Application.Helpers;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;

using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;

namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.Login;

public class LoginHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IPasswordHashService _passwordHashService;
    private readonly IRedisService _redisService;
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;

    public LoginHandler(
        IPasswordHashService passwordHashService,
        IRedisService redisService,
        ITokenService tokenService,
        IUserRepository userRepository)
    {
        _passwordHashService = passwordHashService;
        _redisService = redisService;
        _tokenService = tokenService;
        _userRepository = userRepository;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserWithRolesByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedException("Invalid credentials.");
        }

        var isCorrectPassword = _passwordHashService.VerifyPassword(request.Password, user.PasswordHash);

        if (!isCorrectPassword)
        {
            throw new UnauthorizedException("Invalid credentials.");
        }

        var (accessToken, refreshToken) = _tokenService.GenerateTokens(user);
        var (_, expiresIn) = _tokenService.IsTokenExpired(refreshToken);

        var key = KeyHelper.UserRefreshToken(user.Id);
        var ttlInSeconds = (int)(expiresIn! - DateTime.UtcNow).Value.TotalSeconds;

        await _redisService.SetAsync(key, refreshToken, ttlInSeconds);

        return new LoginResponse { AccessToken = accessToken, RefreshToken = refreshToken };
    }
}
