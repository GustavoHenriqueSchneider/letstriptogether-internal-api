using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs.Requests.Admin.User;

public class AdminUpdateUserRequest
{
    // TODO: nao pode ser string vazia quando informado no body
    [MaxLength(150)]
    public string? Name { get; init; }
}
