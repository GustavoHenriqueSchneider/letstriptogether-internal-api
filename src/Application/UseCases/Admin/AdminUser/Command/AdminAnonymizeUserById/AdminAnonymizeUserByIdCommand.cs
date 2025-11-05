using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.AdminUser.Command.AdminAnonymizeUserById;

public class AdminAnonymizeUserByIdCommand : IRequest
{
    public Guid UserId { get; init; }
}
