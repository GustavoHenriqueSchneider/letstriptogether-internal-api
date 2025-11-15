namespace Application.UseCases.GroupMember.Query.GetOtherGroupMembersById;

public class GetOtherGroupMembersByIdResponse
{
    public IEnumerable<GetOtherGroupMembersByIdResponseData> Data { get; init; } = [];
    public int Hits { get; init; }
}

public class GetOtherGroupMembersByIdResponseData
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
}
