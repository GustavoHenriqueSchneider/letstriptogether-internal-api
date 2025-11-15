using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Domain.Aggregates.UserAggregate;
using Domain.Common;
using MediatR;

namespace Application.UseCases.User.Command.ChangeCurrentUserPassword;

public class ChangeCurrentUserPasswordHandler(
    IPasswordHashService passwordHashService,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository)
    : IRequestHandler<ChangeCurrentUserPasswordCommand>
{
    public async Task Handle(ChangeCurrentUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        var isCorrectPassword = passwordHashService.VerifyPassword(request.CurrentPassword, user.PasswordHash);
        if (!isCorrectPassword)
        {
            throw new UnauthorizedException("Invalid current password.");
        }

        var newPasswordHash = passwordHashService.HashPassword(request.NewPassword);
        user.SetPasswordHash(newPasswordHash);

        userRepository.Update(user);
        await unitOfWork.SaveAsync(cancellationToken);
    }
}
