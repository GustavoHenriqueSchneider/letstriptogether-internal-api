using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs.Requests.GroupMember;

public class RemoveGroupMemberByIdRequest
{
    [Required(ErrorMessage = "UserId is required")]
    public Guid UserId { get; init; }
}