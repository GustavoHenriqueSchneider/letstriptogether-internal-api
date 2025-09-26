namespace WebApi.Context;

public interface IApplicationUserContext
{
    Guid GetId();
    string GetName();
    string GetEmail();
    string GetRegisterStep();
}
