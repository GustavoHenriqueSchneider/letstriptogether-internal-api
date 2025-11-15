using MediatR;

namespace Application.UseCases.v1.Admin.AdminUser.Command.AdminAnonymizeUserById;

public class AdminAnonymizeUserByIdCommand : IRequest
{
    public Guid UserId { get; init; }
}
