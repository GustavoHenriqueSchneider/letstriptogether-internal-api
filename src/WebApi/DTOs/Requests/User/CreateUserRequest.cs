using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs.Requests.User;

public class CreateUserRequest
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(150)]
    public string Name { get; init; } = null!;

    [Required(ErrorMessage = "Email is required")]
    [MaxLength(254)]
    [EmailAddress]
    public string Email { get; init; } = null!;

    // TODO: verificar regras de senha
    [Required(ErrorMessage = "Password is required")]
    [MinLength(8)]
    [MaxLength(30)]
    public string Password { get; init; } = null!;
}
