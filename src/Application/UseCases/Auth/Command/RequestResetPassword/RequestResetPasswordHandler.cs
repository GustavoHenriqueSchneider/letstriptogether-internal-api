using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Application.Helpers;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.RequestResetPassword;

public class RequestResetPasswordHandler : IRequestHandler<RequestResetPasswordCommand>
{
    private readonly IEmailSenderService _emailSenderService;
    private readonly IRedisService _redisService;
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;

    public RequestResetPasswordHandler(
        IEmailSenderService emailSenderService,
        IRedisService redisService,
        ITokenService tokenService,
        IUserRepository userRepository)
    {
        _emailSenderService = emailSenderService;
        _redisService = redisService;
        _tokenService = tokenService;
        _userRepository = userRepository;
    }

    public async Task Handle(RequestResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            return;
        }

        var resetPasswordToken = _tokenService.GenerateResetPasswordToken(user.Id);
        var (_, expiresIn) = _tokenService.IsTokenExpired(resetPasswordToken);

        var key = KeyHelper.UserResetPassword(user.Id);
        var ttlInSeconds = (int)(expiresIn! - DateTime.UtcNow).Value.TotalSeconds;

        await _redisService.SetAsync(key, resetPasswordToken, ttlInSeconds);
        // TODO: tirar valor hard coded e criar templates de email
        await _emailSenderService.SendAsync(request.Email, "Reset Password", resetPasswordToken, cancellationToken);
    }
}
