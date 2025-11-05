using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.User.Command.SetCurrentUserPreferences;

public class SetCurrentUserPreferencesCommand : IRequest
{
    public Guid UserId { get; init; }
    public bool LikesCommercial { get; init; }
    public List<string> Food { get; init; } = [];
    public List<string> Culture { get; init; } = [];
    public List<string> Entertainment { get; init; } = [];
    public List<string> PlaceTypes { get; init; } = [];
}
