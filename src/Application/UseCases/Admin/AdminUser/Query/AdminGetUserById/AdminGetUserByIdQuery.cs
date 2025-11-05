using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Query.AdminGetUserById;

public class AdminGetUserByIdQuery : IRequest<AdminGetUserByIdResponse>
{
    public Guid UserId { get; init; }
}
