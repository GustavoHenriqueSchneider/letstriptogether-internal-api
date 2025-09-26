namespace WebApi.DTOs.Responses;

public class BaseResponse
{
    public string Status { get; init; } = null!;
    public string? Message { get; init; }
    public object? Data { get; init; }
}
