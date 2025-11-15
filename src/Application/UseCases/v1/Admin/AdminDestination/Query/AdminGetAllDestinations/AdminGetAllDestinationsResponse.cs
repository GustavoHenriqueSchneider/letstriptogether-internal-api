namespace Application.UseCases.v1.Admin.AdminDestination.Query.AdminGetAllDestinations;

public class AdminGetAllDestinationsResponse
{
    public IEnumerable<AdminGetAllDestinationsResponseData> Data { get; init; } = [];
    public int Hits { get; init; }
}

public class AdminGetAllDestinationsResponseData
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
}
