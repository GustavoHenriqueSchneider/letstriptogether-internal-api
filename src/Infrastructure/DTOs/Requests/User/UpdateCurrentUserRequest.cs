using System.ComponentModel.DataAnnotations;

namespace LetsTripTogether.InternalApi.Infrastructure.DTOs.Requests.User;

public class UpdateCurrentUserRequest
{
    // TODO: nao pode ser string vazia quando informado no body
    [MaxLength(150)]
    public string? Name { get; init; }
}
