namespace Application.UseCases.Health.Query.GetHealth;

public class GetHealthResponse
{
    public string Status { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public IEnumerable<HealthCheckData> Checks { get; init; } = [];
}

public class HealthCheckData
{
    public string Name { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string? Description { get; init; }
    public double Duration { get; init; }
    public string? Exception { get; init; }
}
