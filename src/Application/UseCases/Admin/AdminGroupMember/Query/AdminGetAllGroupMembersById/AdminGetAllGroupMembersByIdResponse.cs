namespace LetsTripTogether.InternalApi.Application.UseCases.AdminGroupMember.Query.AdminGetAllGroupMembersById;

public class AdminGetAllGroupMembersByIdResponse
{
    public IEnumerable<AdminGetAllGroupMembersByIdResponseData> Data { get; init; } = [];
    public int Hits { get; init; }
}

public class AdminGetAllGroupMembersByIdResponseData
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
}
