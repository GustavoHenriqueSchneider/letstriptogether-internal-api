using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs.Requests.User;

public class SetPreferencesForCurrentUserRequest
{
    // TODO: nao pode ser string vazia quando informado no body
    [Required(ErrorMessage = "Categories are required.")]
    public List<string> Categories { get; set; } = [];
}
