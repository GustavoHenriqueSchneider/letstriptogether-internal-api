using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using Crypt = BCrypt.Net.BCrypt;

namespace LetsTripTogether.InternalApi.Infrastructure.Services;

public class PasswordHashService : IPasswordHashService
{
    public string HashPassword(string password)
    {
        return Crypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return Crypt.Verify(password, passwordHash);
    }
}
