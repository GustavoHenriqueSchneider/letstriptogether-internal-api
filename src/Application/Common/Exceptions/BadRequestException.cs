using Microsoft.AspNetCore.Http;

namespace Application.Common.Exceptions;

public class BadRequestException : BaseException
{
    public BadRequestException(string message, Exception? innerException = null)
        : base(message, StatusCodes.Status400BadRequest, "Bad Request", innerException)
    {
    }
}
