using System.Security.Claims;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Application.Helpers;
using Domain.Security;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.v1.Auth.Command.ValidateRegisterConfirmationCode;

public class ValidateRegisterConfirmationCodeHandler(
    IRedisService redisService,
    ITokenService tokenService)
    : IRequestHandler<ValidateRegisterConfirmationCodeCommand, ValidateRegisterConfirmationCodeResponse>
{
    public async Task<ValidateRegisterConfirmationCodeResponse> Handle(ValidateRegisterConfirmationCodeCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.ToLowerInvariant();
        var key = KeyHelper.RegisterEmailConfirmation(normalizedEmail);
        var code = await redisService.GetAsync(key);

        if (code is null || code != request.Code.ToString())
        {
            throw new BadRequestException("Invalid code.");
        }

        var claims = new List<Claim>
        {
            new (Claims.Name, request.Name),
            new (ClaimTypes.Email, normalizedEmail)
        };

        var token = tokenService.GenerateRegisterTokenForStep(Step.SetPassword, claims);
        await redisService.DeleteAsync(key);

        return new ValidateRegisterConfirmationCodeResponse { Token = token };
    }
}
