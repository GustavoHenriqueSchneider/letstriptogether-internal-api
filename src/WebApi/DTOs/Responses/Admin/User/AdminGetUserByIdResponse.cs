using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs.Responses.Admin.User;

public class AdminGetUserByIdResponse
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public AdminGetUserByIdPreferenceResponse? Preferences { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public class AdminGetUserByIdPreferenceResponse
{
    public bool LikesCommercial { get; init; }
    public IEnumerable<string> Food { get; init; } = [];
    public IEnumerable<string> Culture { get; init; } = [];
    public IEnumerable<string> Entertainment { get; init; } = [];
    public IEnumerable<string> PlaceTypes { get; init; } = [];
}
