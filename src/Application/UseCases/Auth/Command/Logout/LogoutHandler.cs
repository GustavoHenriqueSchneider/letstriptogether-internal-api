using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Application.Helpers;
using Domain.Aggregates.UserAggregate;
using MediatR;

namespace Application.UseCases.Auth.Command.Logout;

public class LogoutHandler(
    IRedisService redisService,
    IUserRepository userRepository)
    : IRequestHandler<LogoutCommand>
{
    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var userId = request.UserId;
        var userExists = await userRepository.ExistsByIdAsync(userId, cancellationToken);

        if (!userExists)
        {
            throw new NotFoundException("User not found.");
        }

        var key = KeyHelper.UserRefreshToken(userId);
        await redisService.DeleteAsync(key);
    }
}
