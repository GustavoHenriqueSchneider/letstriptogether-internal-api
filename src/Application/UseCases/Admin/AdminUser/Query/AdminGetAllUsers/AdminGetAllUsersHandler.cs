using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Query.AdminGetAllUsers;

public class AdminGetAllUsersHandler(IUserRepository userRepository)
    : IRequestHandler<AdminGetAllUsersQuery, AdminGetAllUsersResponse>
{
    public async Task<AdminGetAllUsersResponse> Handle(AdminGetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var (users, hits) = await userRepository.GetAllAsync(request.PageNumber, request.PageSize, cancellationToken);

        return new AdminGetAllUsersResponse
        {
            Data = users.Select(x => new AdminGetAllUsersResponseData { Id = x.Id, CreatedAt = x.CreatedAt }),
            Hits = hits
        };
    }
}
