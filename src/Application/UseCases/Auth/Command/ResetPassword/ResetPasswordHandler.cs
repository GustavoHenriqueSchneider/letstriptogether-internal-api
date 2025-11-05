using LetsTripTogether.InternalApi.Application.Helpers;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Common;

using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;

namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.ResetPassword;

public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand>
{
    private readonly IPasswordHashService _passwordHashService;
    private readonly IRedisService _redisService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public ResetPasswordHandler(
        IPasswordHashService passwordHashService,
        IRedisService redisService,
        IUnitOfWork unitOfWork,
        IUserRepository userRepository)
    {
        _passwordHashService = passwordHashService;
        _redisService = redisService;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        var key = KeyHelper.UserResetPassword(user.Id);
        var storedResetPasswordToken = await _redisService.GetAsync(key);

        if (storedResetPasswordToken is null || storedResetPasswordToken != request.BearerToken)
        {
            throw new UnauthorizedException("Invalid reset password token.");
        }

        var passwordHash = _passwordHashService.HashPassword(request.Password);

        user.SetPasswordHash(passwordHash);
        _userRepository.Update(user);

        await _unitOfWork.SaveAsync(cancellationToken);
        await _redisService.DeleteAsync(key);
    }
}
