using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs.Requests.Auth;

public class LoginRequest
{
    [Required(ErrorMessage = "Email is required")]
    [MaxLength(254)]
    [EmailAddress]
    public string Email { get; init; } = null!;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8)]
    [MaxLength(30)]
    public string Password { get; init; } = null!;
}
