using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Command.AdminUpdateUserById;

public record AdminUpdateUserByIdCommand : IRequest
{
    public Guid UserId { get; init; }
    public string? Name { get; init; }
}
