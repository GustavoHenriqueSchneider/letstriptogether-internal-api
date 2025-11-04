namespace LetsTripTogether.InternalApi.Domain.Common.Exceptions;

public class DomainBusinessRuleException : Exception
{
    public int StatusCode { get; } = 422; // Unprocessable Entity
    
    public string? Title { get; }

    public DomainBusinessRuleException(string message, string? title = null, Exception? innerException = null)
        : base(message, innerException)
    {
        Title = title ?? "Business Rule Violation";
    }
}
