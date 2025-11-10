using MediatR;

namespace Application.UseCases.Auth.Command.RefreshToken;

public class RefreshTokenCommand : IRequest<RefreshTokenResponse>
{
    public string RefreshToken { get; init; } = null!;
}
