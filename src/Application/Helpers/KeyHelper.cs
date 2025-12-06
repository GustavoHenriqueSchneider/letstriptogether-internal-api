namespace Application.Helpers;

public static class KeyHelper
{
    public static string RegisterEmailConfirmation(string email) => $"auth:register:email-confirmation:{email.ToLowerInvariant()}";
    public static string UserRefreshToken(Guid userId) => $"auth:user:refresh-token:{userId.ToString()}";
    public static string UserResetPassword(Guid userId) => $"auth:user:reset-password:{userId.ToString()}";
}
