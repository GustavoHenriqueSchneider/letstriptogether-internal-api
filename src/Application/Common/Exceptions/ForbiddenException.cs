using Microsoft.AspNetCore.Http;

namespace LetsTripTogether.InternalApi.Application.Common.Exceptions;

public class ForbiddenException : BaseException
{
    public ForbiddenException(string message = "Forbidden", Exception? innerException = null)
        : base(message, StatusCodes.Status403Forbidden, "Forbidden", innerException)
    {
    }
}
