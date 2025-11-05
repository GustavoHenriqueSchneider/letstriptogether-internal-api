using LetsTripTogether.InternalApi.Application.Helpers;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;

using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;

namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.Logout;

public class LogoutHandler : IRequestHandler<LogoutCommand>
{
    private readonly IRedisService _redisService;
    private readonly IUserRepository _userRepository;

    public LogoutHandler(
        IRedisService redisService,
        IUserRepository userRepository)
    {
        _redisService = redisService;
        _userRepository = userRepository;
    }

    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var userId = request.UserId;
        var userExists = await _userRepository.ExistsByIdAsync(userId, cancellationToken);

        if (!userExists)
        {
            throw new NotFoundException("User not found.");
        }

        var key = KeyHelper.UserRefreshToken(userId);
        await _redisService.DeleteAsync(key);

        // TODO: fazer logica pra travar accesstoken usado no logout enquanto ainda nao expirar
    }
}
