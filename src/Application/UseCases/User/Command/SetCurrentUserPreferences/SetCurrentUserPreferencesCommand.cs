using System.Text.Json.Serialization;
using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.User.Command.SetCurrentUserPreferences;

public record SetCurrentUserPreferencesCommand : IRequest
{
    [JsonIgnore] public Guid UserId { get; init; }
    public bool LikesCommercial { get; init; }
    public List<string> Food { get; init; } = [];
    public List<string> Culture { get; init; } = [];
    public List<string> Entertainment { get; init; } = [];
    public List<string> PlaceTypes { get; init; } = [];
}
