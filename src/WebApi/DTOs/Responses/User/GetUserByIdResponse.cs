namespace WebApi.DTOs.Responses.User;

public class GetUserByIdResponse
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public GetUserByIdPreferenceResponse Preferences { get; init; } = new();
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public class GetUserByIdPreferenceResponse
{
    public List<string> Categories { get; set; } = [];
}
