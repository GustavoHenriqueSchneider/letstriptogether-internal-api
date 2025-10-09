using WebApi.Services.Interfaces;
using Crypt = BCrypt.Net.BCrypt;

namespace WebApi.Services.Implementations;

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
