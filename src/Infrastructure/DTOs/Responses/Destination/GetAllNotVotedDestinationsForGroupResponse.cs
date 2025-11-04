namespace LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses.Destination;

public class GetAllNotVotedDestinationsForGroupResponse 
    : PaginatedResponse<GetAllNotVotedDestinationsForGroupResponseData>
{
}

public class GetAllNotVotedDestinationsForGroupResponseData
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
}
