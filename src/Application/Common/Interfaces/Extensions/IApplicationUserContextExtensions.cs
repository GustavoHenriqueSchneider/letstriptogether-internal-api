namespace LetsTripTogether.InternalApi.Application.Common.Interfaces.Extensions;

public interface IApplicationUserContextExtensions
{
    Guid GetId();
    string GetName();
    string GetEmail();
    string GetRegisterStep();
}

