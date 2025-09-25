using System.Security.Claims;
using WebApi.Security;

namespace WebApi.Context;

public class ApplicationUserContext(ClaimsPrincipal principal) : IApplicationUserContext
{
    public Guid GetId()
    {
        var id = principal.FindFirstValue(Claims.Id);
        return Guid.TryParse(id, out var guid) ? guid : Guid.Empty;
    }
   
    public string GetName()
    {
        return principal.FindFirstValue(Claims.Name) ?? string.Empty;
    }

    public string GetEmail()
    {
        return principal.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
    }

    public string GetRegisterStep()
    {
        return principal.FindFirstValue(Claims.Step) ?? string.Empty;
    }
}
