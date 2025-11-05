using System.Security.Claims;
using LetsTripTogether.InternalApi.Application.Helpers;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Security;
using LetsTripTogether.InternalApi.Domain.ValueObjects;
using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;

namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.SendRegisterConfirmationEmail;

public class SendRegisterConfirmationEmailHandler(
    IEmailSenderService emailSenderService,
    IRandomCodeGeneratorService randomCodeGeneratorService,
    IRedisService redisService,
    ITokenService tokenService,
    IUserRepository userRepository)
    : IRequestHandler<SendRegisterConfirmationEmailCommand, SendRegisterConfirmationEmailResponse>
{
    public async Task<SendRegisterConfirmationEmailResponse> Handle(SendRegisterConfirmationEmailCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email;
        var existsUserWithEmail = await userRepository.ExistsByEmailAsync(email, cancellationToken);

        if (existsUserWithEmail)
        {
            throw new ConflictException("There is already an user using this email.");
        }

        var claims = new List<Claim>
        {
            new (Claims.Name, request.Name),
            new (ClaimTypes.Email, email)
        };

        var token = tokenService.GenerateRegisterTokenForStep(Step.ValidateEmail, claims);
        var (_, expiresIn) = tokenService.IsTokenExpired(token);

        var key = KeyHelper.RegisterEmailConfirmation(email);
        var ttlInSeconds = (int)(expiresIn! - DateTime.UtcNow).Value.TotalSeconds;

        var code = randomCodeGeneratorService.Generate();
        await redisService.SetAsync(key, code, ttlInSeconds);
        // TODO: tirar valor hard coded e criar templates de email
        await emailSenderService.SendAsync(request.Email, "Email Confirmation", code, cancellationToken);

        return new SendRegisterConfirmationEmailResponse { Token = token };
    }
}
