using Application.Common.Interfaces.Extensions;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Extensions;

public class HttpContextExtensions(IHttpContextAccessor httpContextAccessor) : IHttpContextExtensions
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
