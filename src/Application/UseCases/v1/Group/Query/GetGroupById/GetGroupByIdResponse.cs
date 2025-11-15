namespace Application.UseCases.v1.Group.Query.GetGroupById;

public class GetGroupByIdResponse
{
    public string Name { get; init; } = null!;
    public DateTime TripExpectedDate { get; init; }
    public bool IsCurrentMemberOwner { get; init; }
    public GetGroupByIdPreferenceResponse Preferences { get; init; } = new();
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public class GetGroupByIdPreferenceResponse
{
    public bool LikesShopping { get; init; }
    public bool LikesGastronomy { get; init; }
    public IEnumerable<string> Culture { get; init; } = null!;
    public IEnumerable<string> Entertainment { get; init; } = null!;
    public IEnumerable<string> PlaceTypes { get; init; } = null!;
}
