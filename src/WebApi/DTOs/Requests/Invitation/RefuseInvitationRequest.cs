using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs.Requests.Invitation;

public class RefuseInvitationRequest
{
    [Required(ErrorMessage = "Token is required")]
    public string Token { get; init; } = null!;
}