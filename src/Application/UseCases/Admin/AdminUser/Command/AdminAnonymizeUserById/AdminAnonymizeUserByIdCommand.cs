using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Command.AdminAnonymizeUserById;

public class AdminAnonymizeUserByIdCommand : IRequest
{
    public Guid UserId { get; init; }
}
