using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs.Requests.GroupMember;

public class RemoveGroupMemberByIdRequest
{
    [Required(ErrorMessage = "MemberId is required")]
    public Guid MemberId { get; init; }
}