using System.ComponentModel.DataAnnotations;

namespace LetsTripTogether.InternalApi.Infrastructure.DTOs.Requests.Admin.User;

public class AdminSetUserPreferencesByUserIdRequest
{
    [Required(ErrorMessage = "Likes commercial preference is required.")]
    public bool LikesCommercial { get; init; }

    [Required(ErrorMessage = "Food preferences are required.")]
    public List<string> Food { get; init; } = [];

    [Required(ErrorMessage = "Culture preferences are required.")]
    public List<string> Culture { get; init; } = [];

    [Required(ErrorMessage = "Entertainment preferences are required.")]
    public List<string> Entertainment { get; init; } = [];

    [Required(ErrorMessage = "Place types are required.")]
    public List<string> PlaceTypes { get; init; } = [];
}
