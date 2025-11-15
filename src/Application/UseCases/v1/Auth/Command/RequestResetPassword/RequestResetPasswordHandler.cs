using Application.Common.Interfaces.Services;
using Application.Helpers;
using Domain.Aggregates.UserAggregate;
using Domain.Security;
using MediatR;

namespace Application.UseCases.v1.Auth.Command.RequestResetPassword;

public class RequestResetPasswordHandler(
    IEmailSenderService emailSenderService,
    IRedisService redisService,
    ITokenService tokenService,
    IUserRepository userRepository)
    : IRequestHandler<RequestResetPasswordCommand>
{
    public async Task Handle(RequestResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            return;
        }

        var resetPasswordToken = tokenService.GenerateResetPasswordToken(user.Id);
        var (_, expiresIn) = tokenService.IsTokenExpired(resetPasswordToken);

        var key = KeyHelper.UserResetPassword(user.Id);
        var ttlInSeconds = (int)(expiresIn! - DateTime.UtcNow).Value.TotalSeconds;

        await redisService.SetAsync(key, resetPasswordToken, ttlInSeconds);

        var expiresInMinutes = (int)(expiresIn! - DateTime.UtcNow).Value.TotalMinutes + 1;
        var templateData = new Dictionary<string, string>
        {
            { "token", resetPasswordToken },
            { "expiresIn", expiresInMinutes.ToString() },
            { "email", request.Email }
        };

        await emailSenderService.SendAsync(request.Email, EmailTemplates.ResetPassword, 
            templateData, cancellationToken);
    }
}
