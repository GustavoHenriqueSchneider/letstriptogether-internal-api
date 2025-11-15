using MediatR;

namespace Application.UseCases.v1.Admin.AdminUser.Command.AdminDeleteUserById;

public class AdminDeleteUserByIdCommand : IRequest
{
    public Guid UserId { get; init; }
}
