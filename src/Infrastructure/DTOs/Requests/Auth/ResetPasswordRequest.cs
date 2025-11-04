using System.ComponentModel.DataAnnotations;

namespace LetsTripTogether.InternalApi.Infrastructure.DTOs.Requests.Auth;

public class ResetPasswordRequest
{
    [Required(ErrorMessage = "Password is required")]
    [MinLength(8)]
    [MaxLength(30)]
    public string Password { get; init; } = null!;
}
