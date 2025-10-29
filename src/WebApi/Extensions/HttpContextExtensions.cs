using Microsoft.AspNetCore.Http;

namespace WebApi.Extensions;

public static class HttpContextExtensions
{
    public static string? GetBearerToken(this HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        return authHeader?.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) == true 
            ? authHeader.Substring("Bearer ".Length).Trim()
            : null;
    }
}
