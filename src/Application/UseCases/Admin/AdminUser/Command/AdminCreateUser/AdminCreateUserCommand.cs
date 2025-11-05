using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.AdminUser.Command.AdminCreateUser;

public class AdminCreateUserCommand : IRequest<AdminCreateUserResponse>
{
    public string Name { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
}
