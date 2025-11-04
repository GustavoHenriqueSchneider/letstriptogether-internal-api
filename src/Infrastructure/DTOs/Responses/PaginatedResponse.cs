namespace LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses;

public class PaginatedResponse<T> where T : class
{
    public IEnumerable<T> Data { get; init; } = [];
    public int Hits { get; init; }
}
