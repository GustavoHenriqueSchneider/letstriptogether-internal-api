using Microsoft.AspNetCore.Http;

namespace Application.Common.Exceptions;

public class UnauthorizedException : BaseException
{
    public UnauthorizedException(string message = "Unauthorized", Exception? innerException = null)
        : base(message, StatusCodes.Status401Unauthorized, "Unauthorized", innerException)
    {
    }
}
