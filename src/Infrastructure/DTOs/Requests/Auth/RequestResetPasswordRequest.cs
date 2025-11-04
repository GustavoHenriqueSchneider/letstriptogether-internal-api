using System.ComponentModel.DataAnnotations;

namespace LetsTripTogether.InternalApi.Infrastructure.DTOs.Requests.Auth;

public class RequestResetPasswordRequest
{
    [Required(ErrorMessage = "Email is required")]
    [MaxLength(254)]
    [EmailAddress]
    public string Email { get; init; } = null!;
}
