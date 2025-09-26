using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs.Requests.Auth;

public class ValidateRegisterConfirmationCodeRequest
{
    // TODO : validar codigo entre 100000 e 999999
    [Required(ErrorMessage = "Code is required")]
    public int Code { get; init; }
}
