using System.Security.Claims;
using LetsTripTogether.InternalApi.Application.Helpers;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Security;
using LetsTripTogether.InternalApi.Domain.ValueObjects;
using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;

namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.SendRegisterConfirmationEmail;

public class SendRegisterConfirmationEmailHandler : IRequestHandler<SendRegisterConfirmationEmailCommand, SendRegisterConfirmationEmailResponse>
{
    private readonly IEmailSenderService _emailSenderService;
    private readonly IRandomCodeGeneratorService _randomCodeGeneratorService;
    private readonly IRedisService _redisService;
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;

    public SendRegisterConfirmationEmailHandler(
        IEmailSenderService emailSenderService,
        IRandomCodeGeneratorService randomCodeGeneratorService,
        IRedisService redisService,
        ITokenService tokenService,
        IUserRepository userRepository)
    {
        _emailSenderService = emailSenderService;
        _randomCodeGeneratorService = randomCodeGeneratorService;
        _redisService = redisService;
        _tokenService = tokenService;
        _userRepository = userRepository;
    }

    public async Task<SendRegisterConfirmationEmailResponse> Handle(SendRegisterConfirmationEmailCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email;
        var existsUserWithEmail = await _userRepository.ExistsByEmailAsync(email, cancellationToken);

        if (existsUserWithEmail)
        {

            throw new ConflictException("There is already an user using this email.");
        }

        var claims = new List<Claim>
        {
            new (Claims.Name, request.Name),
            new (ClaimTypes.Email, email)
        };

        var token = _tokenService.GenerateRegisterTokenForStep(Step.ValidateEmail, claims);
        var (_, expiresIn) = _tokenService.IsTokenExpired(token);

        var key = KeyHelper.RegisterEmailConfirmation(email);
        var ttlInSeconds = (int)(expiresIn! - DateTime.UtcNow).Value.TotalSeconds;

        var code = _randomCodeGeneratorService.Generate();
        await _redisService.SetAsync(key, code, ttlInSeconds);
        // TODO: tirar valor hard coded e criar templates de email
        await _emailSenderService.SendAsync(request.Email, "Email Confirmation", code, cancellationToken);

        return new SendRegisterConfirmationEmailResponse { Token = token };
    }
}
