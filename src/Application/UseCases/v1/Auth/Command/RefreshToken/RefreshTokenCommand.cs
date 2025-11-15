using MediatR;

namespace Application.UseCases.v1.Auth.Command.RefreshToken;

public class RefreshTokenCommand : IRequest<RefreshTokenResponse>
{
    public string RefreshToken { get; init; } = null!;
}
