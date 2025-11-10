using Microsoft.AspNetCore.Http;

namespace Application.Common.Exceptions;

public class ConflictException : BaseException
{
    public ConflictException(string message, Exception? innerException = null)
        : base(message, StatusCodes.Status409Conflict, "Conflict", innerException)
    {
    }
}
