namespace LetsTripTogether.InternalApi.Application.Common.Exceptions;

public abstract class BaseException : Exception
{
    public int StatusCode { get; }
    
    public string? Title { get; }

    protected BaseException(
        string message,
        int statusCode,
        string? title = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        Title = title;
    }
}
