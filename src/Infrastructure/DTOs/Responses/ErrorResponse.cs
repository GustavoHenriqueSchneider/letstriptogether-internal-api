namespace LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses;

// TODO: pode virar exception pra ficar mais clean nos handlers e ter middleware tratando
public class ErrorResponse(string message)
{
    public string Message { get; private set; } = message;
}
