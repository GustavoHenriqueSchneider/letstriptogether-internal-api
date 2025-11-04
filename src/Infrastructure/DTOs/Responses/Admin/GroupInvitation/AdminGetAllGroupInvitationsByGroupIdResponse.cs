namespace LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses.Admin.GroupInvitation;

public class AdminGetAllGroupInvitationsByGroupIdResponse 
    : PaginatedResponse<AdminGetAllGroupInvitationsByGroupIdResponseData>
{
}

public class AdminGetAllGroupInvitationsByGroupIdResponseData
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
}

