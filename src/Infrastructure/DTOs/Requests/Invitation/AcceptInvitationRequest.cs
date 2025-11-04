using System.ComponentModel.DataAnnotations;

namespace LetsTripTogether.InternalApi.Infrastructure.DTOs.Requests.Invitation;

public class AcceptInvitationRequest
{
    [Required(ErrorMessage = "Token is required")]
    public string Token { get; init; } = null!;
}