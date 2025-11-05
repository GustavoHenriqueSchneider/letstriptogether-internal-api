namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupInvitation.Query.AdminGetAllGroupInvitationsByGroupId;

public class AdminGetAllGroupInvitationsByGroupIdResponse
{
    public IEnumerable<AdminGetAllGroupInvitationsByGroupIdResponseData> Data { get; init; } = [];
    public int Hits { get; init; }
}

public class AdminGetAllGroupInvitationsByGroupIdResponseData
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
}
