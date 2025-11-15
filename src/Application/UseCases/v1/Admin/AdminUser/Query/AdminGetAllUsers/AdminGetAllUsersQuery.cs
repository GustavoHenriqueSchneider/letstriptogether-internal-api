using MediatR;

namespace Application.UseCases.v1.Admin.AdminUser.Query.AdminGetAllUsers;

public class AdminGetAllUsersQuery : IRequest<AdminGetAllUsersResponse>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
