using System.Security.Claims;
using LetsTripTogether.InternalApi.Application.Helpers;
using LetsTripTogether.InternalApi.Domain.Security;
using LetsTripTogether.InternalApi.Domain.ValueObjects;
using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;

namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.ValidateRegisterConfirmationCode;

public class ValidateRegisterConfirmationCodeHandler(
    IRedisService redisService,
    ITokenService tokenService)
    : IRequestHandler<ValidateRegisterConfirmationCodeCommand, ValidateRegisterConfirmationCodeResponse>
{
    public async Task<ValidateRegisterConfirmationCodeResponse> Handle(ValidateRegisterConfirmationCodeCommand request, CancellationToken cancellationToken)
    {
        var key = KeyHelper.RegisterEmailConfirmation(request.Email);
        var code = await redisService.GetAsync(key);

        if (code is null || code != request.Code)
        {
            throw new BadRequestException("Invalid code.");
        }

        var claims = new List<Claim>
        {
            new (Claims.Name, request.Name),
            new (ClaimTypes.Email, request.Email)
        };

        var token = tokenService.GenerateRegisterTokenForStep(Step.SetPassword, claims);
        await redisService.DeleteAsync(key);

        return new ValidateRegisterConfirmationCodeResponse { Token = token };
    }
}
