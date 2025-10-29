namespace WebApi.DTOs.Responses.Destination;

public class GetAllDestinationsResponse : PaginatedResponse<GetAllDestinationsResponseData>
{
}

public class GetAllDestinationsResponseData
{
    public Guid Id { get; init; }
    public string Address { get; init; }
    public DateTime CreatedAt { get; init; }
}
