using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs.Requests.User;

public class UpdateForCurrentUserRequest
{
    // TODO: nao pode ser string vazia quando informado no body
    [MaxLength(150)]
    public string? Name { get; init; }
}
