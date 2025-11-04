using Microsoft.AspNetCore.Http;

namespace LetsTripTogether.InternalApi.Application.Common.Extensions;

public class HttpContextExtensions(IHttpContextAccessor httpContextAccessor)
{
    public string? GetBearerToken()
    {
        var authHeader = httpContextAccessor.HttpContext.Request
            .Headers["Authorization"].FirstOrDefault();
        
        return authHeader?.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) == true 
            ? authHeader.Substring("Bearer ".Length).Trim()
            : null;
    }
}
