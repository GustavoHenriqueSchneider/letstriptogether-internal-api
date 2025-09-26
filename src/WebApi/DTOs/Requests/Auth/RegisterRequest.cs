using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs.Requests.Auth;

public class RegisterRequest
{
    // TODO: verificar regras de senha
    [Required(ErrorMessage = "Password is required")]
    [MinLength(8)]
    [MaxLength(30)]
    public string Password { get; init; } = null!;
}
