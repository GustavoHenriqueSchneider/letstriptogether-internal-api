namespace WebApi.DTOs.Responses.Admin.User;

public class AdminGetUserByIdResponse
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public AdminGetUserByIdPreferenceResponse Preferences { get; init; } = new();
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public class AdminGetUserByIdPreferenceResponse
{
    public List<string> Categories { get; set; } = [];
}
