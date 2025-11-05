using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.AdminUser.Command.AdminDeleteUserById;

public class AdminDeleteUserByIdCommand : IRequest
{
    public Guid UserId { get; init; }
}
