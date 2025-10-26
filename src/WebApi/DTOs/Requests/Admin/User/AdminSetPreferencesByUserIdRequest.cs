using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs.Requests.Admin.User;

public class AdminSetUserPreferencesByUserIdRequest
{
    // TODO: nao pode ser string vazia quando informado no body
    [Required(ErrorMessage = "Categories are required.")]
    public List<string> Categories { get; set; } = [];
}
