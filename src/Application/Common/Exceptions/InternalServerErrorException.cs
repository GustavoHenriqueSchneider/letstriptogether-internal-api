using Microsoft.AspNetCore.Http;

namespace LetsTripTogether.InternalApi.Application.Common.Exceptions;

public class InternalServerErrorException : BaseException
{
    public InternalServerErrorException(string message, Exception? innerException = null)
        : base(message, StatusCodes.Status500InternalServerError, "Internal Server Error", innerException)
    {
    }
}
