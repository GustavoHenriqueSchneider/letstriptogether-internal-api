using System.Security.Claims;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Application.Helpers;
using Domain.Security;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.Auth.Command.ValidateRegisterConfirmationCode;

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
