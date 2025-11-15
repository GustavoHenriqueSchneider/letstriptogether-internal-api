using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Application.Helpers;
using Domain.Aggregates.UserAggregate;
using Domain.Common;
using MediatR;

namespace Application.UseCases.v1.Auth.Command.ResetPassword;

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
