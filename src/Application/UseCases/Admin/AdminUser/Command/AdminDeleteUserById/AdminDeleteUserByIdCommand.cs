using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Command.AdminDeleteUserById;

public class AdminDeleteUserByIdCommand : IRequest
{
    public Guid UserId { get; init; }
}
