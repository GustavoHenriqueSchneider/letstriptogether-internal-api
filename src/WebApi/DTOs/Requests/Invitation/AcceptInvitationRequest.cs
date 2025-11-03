using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs.Requests.Invitation;

public class AcceptInvitationRequest
{
    [Required(ErrorMessage = "Token is required")]
    public string Token { get; init; } = null!;
}