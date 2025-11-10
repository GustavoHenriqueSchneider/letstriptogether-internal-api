using System.Security.Claims;
using Application.Common.Interfaces.Extensions;
using Domain.Security;

namespace Application.Common.Extensions;

public class ApplicationUserContextExtensions(ClaimsPrincipal principal) 
    : IApplicationUserContextExtensions
{
    public Guid GetId()
    {
        var id = principal.FindFirst(Claims.Id)?.Value;
        return Guid.TryParse(id, out var guid) ? guid : Guid.Empty;
    }
    
    public string GetName()
    {
        return principal.FindFirst(Claims.Name)?.Value ?? string.Empty;
    }

    public string GetEmail()
    {
        return principal.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
    }

    public string GetRegisterStep()
    {
        return principal.FindFirst(Claims.Step)?.Value ?? string.Empty;
    }
}
