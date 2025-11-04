using System.ComponentModel.DataAnnotations;

namespace LetsTripTogether.InternalApi.Infrastructure.DTOs.Requests.Auth;

public class SendRegisterConfirmationEmailRequest
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(150)]
    public string Name { get; init; } = null!;

    [Required(ErrorMessage = "Email is required")]
    [MaxLength(254)]
    [EmailAddress]
    public string Email { get; init; } = null!;
}
