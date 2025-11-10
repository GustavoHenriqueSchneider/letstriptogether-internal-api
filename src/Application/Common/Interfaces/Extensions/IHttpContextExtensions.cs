namespace Application.Common.Interfaces.Extensions;

public interface IHttpContextExtensions
{
    string? GetBearerToken();
}

