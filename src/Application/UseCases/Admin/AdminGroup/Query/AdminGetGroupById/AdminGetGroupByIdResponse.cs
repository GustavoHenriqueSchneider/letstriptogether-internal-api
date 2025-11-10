namespace Application.UseCases.Admin.AdminGroup.Query.AdminGetGroupById;

public class AdminGetGroupByIdResponse
{
    public string Name { get; init; } = null!;
    public DateTime TripExpectedDate { get; init; }
    public AdminGetGroupByIdPreferenceResponse Preferences { get; init; } = new();
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public class AdminGetGroupByIdPreferenceResponse
{
    public bool LikesCommercial { get; init; }
    public IEnumerable<string> Food { get; init; } = [];
    public IEnumerable<string> Culture { get; init; } = [];
    public IEnumerable<string> Entertainment { get; init; } = [];
    public IEnumerable<string> PlaceTypes { get; init; } = [];
}
