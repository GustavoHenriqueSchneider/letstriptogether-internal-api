namespace WebApi.DTOs.Requests.GroupMember;

public record AddUserToGroupRequest(Guid UserId, bool IsOwner = false);
