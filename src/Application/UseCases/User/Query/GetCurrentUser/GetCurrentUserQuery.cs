using System.Text.Json.Serialization;
using MediatR;

namespace Application.UseCases.User.Query.GetCurrentUser;

public class GetCurrentUserQuery : IRequest<GetCurrentUserResponse>
{
    [JsonIgnore] public Guid UserId { get; init; }
}
