namespace WebApi.DTOs.Responses.User;

public class GetCurrentUserResponse
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public GetCurrentUserPreferenceResponse Preferences { get; init; } = new();
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public class GetCurrentUserPreferenceResponse
{
    public List<string> Categories { get; set; } = [];
}
