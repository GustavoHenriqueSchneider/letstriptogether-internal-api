namespace LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses;

public class BaseResponse(string message)
{
    public string Message { get; private set; } = message;
}
