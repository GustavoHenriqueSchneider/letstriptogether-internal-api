using Microsoft.AspNetCore.Http;

namespace LetsTripTogether.InternalApi.Application.Common.Exceptions;

public class ValidationException : BaseException
{
    public Dictionary<string, string[]> Errors { get; }

    public ValidationException(Dictionary<string, string[]> errors, Exception? innerException = null)
        : base("One or more validation errors occurred.", StatusCodes.Status400BadRequest, "Validation Error", innerException)
    {
        Errors = errors;
    }
}
