using System.Security.Claims;
using LetsTripTogether.InternalApi.Application.Helpers;
using LetsTripTogether.InternalApi.Domain.Security;
using LetsTripTogether.InternalApi.Domain.ValueObjects;

using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;

namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.ValidateRegisterConfirmationCode;

public class ValidateRegisterConfirmationCodeHandler : IRequestHandler<ValidateRegisterConfirmationCodeCommand, ValidateRegisterConfirmationCodeResponse>
{
    private readonly IRedisService _redisService;
    private readonly ITokenService _tokenService;

    public ValidateRegisterConfirmationCodeHandler(
        IRedisService redisService,
        ITokenService tokenService)
    {
        _redisService = redisService;
        _tokenService = tokenService;
    }

    public async Task<ValidateRegisterConfirmationCodeResponse> Handle(ValidateRegisterConfirmationCodeCommand request, CancellationToken cancellationToken)
    {
        var key = KeyHelper.RegisterEmailConfirmation(request.Email);
        var code = await _redisService.GetAsync(key);

        if (code is null || code != request.Code)
        {
            throw new BadRequestException("Invalid code.");
        }

        var claims = new List<Claim>
        {
            new (Claims.Name, request.Name),
            new (ClaimTypes.Email, request.Email)
        };

        var token = _tokenService.GenerateRegisterTokenForStep(Step.SetPassword, claims);
        await _redisService.DeleteAsync(key);

        return new ValidateRegisterConfirmationCodeResponse { Token = token };
    }
}
