using Microsoft.AspNetCore.Http;

namespace LetsTripTogether.InternalApi.Application.Common.Exceptions;

public class NotFoundException : BaseException
{
    public NotFoundException(string message, Exception? innerException = null)
        : base(message, StatusCodes.Status404NotFound, "Not Found", innerException)
    {
    }
}
