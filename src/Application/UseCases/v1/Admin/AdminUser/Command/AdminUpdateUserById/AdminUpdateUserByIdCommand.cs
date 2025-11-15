using MediatR;

namespace Application.UseCases.v1.Admin.AdminUser.Command.AdminUpdateUserById;

public record AdminUpdateUserByIdCommand : IRequest
{
    public Guid UserId { get; init; }
    public string? Name { get; init; }
}
