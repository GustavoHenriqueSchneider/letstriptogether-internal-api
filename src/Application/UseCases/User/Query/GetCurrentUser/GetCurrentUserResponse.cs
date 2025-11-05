namespace LetsTripTogether.InternalApi.Application.UseCases.User.Query.GetCurrentUser;

public class GetCurrentUserPreferenceResponse
{
    public bool LikesCommercial { get; init; }
    public IEnumerable<string> Food { get; init; } = null!;
    public IEnumerable<string> Culture { get; init; } = null!;
    public IEnumerable<string> Entertainment { get; init; } = null!;
    public IEnumerable<string> PlaceTypes { get; init; } = null!;
}

public class GetCurrentUserResponse
{
    public string Name { get; init; } = null!;
    public string Email { get; init; } = null!;
    public GetCurrentUserPreferenceResponse? Preferences { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
