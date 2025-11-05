using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.AdminUser.Command.AdminUpdateUserById;

public class AdminUpdateUserByIdCommand : IRequest
{
    public Guid UserId { get; init; }
    public string? Name { get; init; }
}
