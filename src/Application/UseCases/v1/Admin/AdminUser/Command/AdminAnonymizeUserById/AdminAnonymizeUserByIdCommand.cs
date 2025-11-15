using MediatR;

namespace Application.UseCases.Admin.AdminUser.Command.AdminAnonymizeUserById;

public class AdminAnonymizeUserByIdCommand : IRequest
{
    public Guid UserId { get; init; }
}
