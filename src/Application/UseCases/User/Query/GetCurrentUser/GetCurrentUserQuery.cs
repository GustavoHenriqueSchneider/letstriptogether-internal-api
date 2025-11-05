using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.User.Query.GetCurrentUser;

public class GetCurrentUserQuery : IRequest<GetCurrentUserResponse>
{
    public Guid UserId { get; init; }
}
