namespace Application.UseCases.Error.Query.GetError;

public class GetErrorResponse
{
    public int Status { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Detail { get; init; }
    public string Instance { get; init; } = string.Empty;
    public Dictionary<string, object>? Extensions { get; init; }
}
