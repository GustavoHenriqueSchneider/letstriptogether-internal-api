using System.Text.Json.Serialization;
using MediatR;

namespace Application.UseCases.Auth.Command.Logout;

public class LogoutCommand : IRequest
{
    [JsonIgnore] public Guid UserId { get; init; }
}
