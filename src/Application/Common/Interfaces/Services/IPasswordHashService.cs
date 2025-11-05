namespace LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;

public interface IPasswordHashService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}
