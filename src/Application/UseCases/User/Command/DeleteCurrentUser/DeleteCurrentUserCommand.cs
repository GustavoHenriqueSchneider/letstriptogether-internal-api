using System.Text.Json.Serialization;
using MediatR;

namespace Application.UseCases.User.Command.DeleteCurrentUser;

public class DeleteCurrentUserCommand : IRequest
{
    [JsonIgnore] public Guid UserId { get; init; }
}
