using System.Text.Json.Serialization;
using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.User.Command.AnonymizeCurrentUser;

public class AnonymizeCurrentUserCommand : IRequest
{
    [JsonIgnore] public Guid UserId { get; init; }
}
