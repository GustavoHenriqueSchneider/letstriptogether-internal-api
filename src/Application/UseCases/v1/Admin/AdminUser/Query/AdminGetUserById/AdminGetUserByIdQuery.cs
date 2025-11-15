using MediatR;

namespace Application.UseCases.v1.Admin.AdminUser.Query.AdminGetUserById;

public class AdminGetUserByIdQuery : IRequest<AdminGetUserByIdResponse>
{
    public Guid UserId { get; init; }
}
