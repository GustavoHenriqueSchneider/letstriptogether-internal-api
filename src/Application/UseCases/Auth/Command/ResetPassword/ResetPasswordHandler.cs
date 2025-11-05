using LetsTripTogether.InternalApi.Application.Helpers;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Common;

using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;

namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.ResetPassword;

public class ResetPasswordHandler(
    IPasswordHashService passwordHashService,
    IRedisService redisService,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository)
    : IRequestHandler<ResetPasswordCommand>
{
    public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        var key = KeyHelper.UserResetPassword(user.Id);
        var storedResetPasswordToken = await redisService.GetAsync(key);

        if (storedResetPasswordToken is null || storedResetPasswordToken != request.BearerToken)
        {
            throw new UnauthorizedException("Invalid reset password token.");
        }

        var passwordHash = passwordHashService.HashPassword(request.Password);

        user.SetPasswordHash(passwordHash);
        userRepository.Update(user);

        await unitOfWork.SaveAsync(cancellationToken);
        await redisService.DeleteAsync(key);
    }
}
