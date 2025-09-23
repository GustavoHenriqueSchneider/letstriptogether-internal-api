using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs.Requests.Auth;

public class RefreshTokenRequest
{
    [Required(ErrorMessage = "RefreshToken is required")]
    public string RefreshToken { get; init; } = null!;
}
