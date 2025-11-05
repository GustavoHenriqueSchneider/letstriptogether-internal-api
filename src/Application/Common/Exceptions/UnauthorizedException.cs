using Microsoft.AspNetCore.Http;

namespace LetsTripTogether.InternalApi.Application.Common.Exceptions;

public class UnauthorizedException : BaseException
{
    public UnauthorizedException(string message = "Unauthorized", Exception? innerException = null)
        : base(message, StatusCodes.Status401Unauthorized, "Unauthorized", innerException)
    {
    }
}
