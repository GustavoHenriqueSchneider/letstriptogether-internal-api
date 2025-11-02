using WebApi.DTOs.Responses;

namespace WebApi.DTOs.Responses.Admin.GroupInvitation;

public class AdminGetAllGroupInvitationsByGroupIdResponse 
    : PaginatedResponse<AdminGetAllGroupInvitationsByGroupIdResponseData>
{
}

public class AdminGetAllGroupInvitationsByGroupIdResponseData
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
}

