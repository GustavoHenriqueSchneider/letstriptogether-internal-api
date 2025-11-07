using System.Text.Json.Serialization;
using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.Logout;

public class LogoutCommand : IRequest
{
    [JsonIgnore] public Guid UserId { get; init; }
}
