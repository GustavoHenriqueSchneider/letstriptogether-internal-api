namespace LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses.Admin.Destination;

public class AdminGetAllDestinationsResponse : PaginatedResponse<AdminGetAllDestinationsResponseData>
{
}

public class AdminGetAllDestinationsResponseData
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
}
