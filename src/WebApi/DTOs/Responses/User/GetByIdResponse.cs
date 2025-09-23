namespace WebApi.DTOs.Responses.User;

public class GetByIdResponse
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public GetByIdUserPreferenceResponse Preferences { get; init; } = new();
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public class GetByIdUserPreferenceResponse
{
    public List<string> Categories { get; set; } = [];
}
