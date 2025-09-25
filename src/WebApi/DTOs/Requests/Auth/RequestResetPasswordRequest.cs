using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs.Requests.Auth;

public class RequestResetPasswordRequest
{
    [Required(ErrorMessage = "Email is required")]
    [MaxLength(254)]
    [EmailAddress]
    public string Email { get; init; } = null!;
}
