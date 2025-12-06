using System.Security.Claims;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Application.Helpers;
using Domain.Aggregates.UserAggregate;
using Domain.Security;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.v1.Auth.Command.SendRegisterConfirmationEmail;

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
        var normalizedEmail = request.Email.ToLowerInvariant();
        var existsUserWithEmail = await userRepository.ExistsByEmailAsync(normalizedEmail, cancellationToken);

        if (existsUserWithEmail)
        {
            throw new ConflictException("There is already an user using this email.");
        }

        var claims = new List<Claim>
        {
            new (Claims.Name, request.Name),
            new (ClaimTypes.Email, normalizedEmail)
        };

        var token = tokenService.GenerateRegisterTokenForStep(Step.ValidateEmail, claims);
        var (_, expiresIn) = tokenService.IsTokenExpired(token);

        var key = KeyHelper.RegisterEmailConfirmation(normalizedEmail);
        var ttlInSeconds = (int)(expiresIn! - DateTime.UtcNow).Value.TotalSeconds;

        var code = randomCodeGeneratorService.Generate();
        await redisService.SetAsync(key, code, ttlInSeconds);

        var expiresInMinutes = (int)(expiresIn! - DateTime.UtcNow).Value.TotalMinutes + 1;
        var templateData = new Dictionary<string, string>
        {
            { "code", code },
            { "name", request.Name },
            { "expiresIn", expiresInMinutes.ToString() },
            { "email", normalizedEmail }
        };

        await emailSenderService.SendAsync(normalizedEmail, EmailTemplates.EmailConfirmation, templateData, cancellationToken);

        return new SendRegisterConfirmationEmailResponse { Token = token };
    }
}
