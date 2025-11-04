namespace LetsTripTogether.InternalApi.Application.Helpers;

public static class KeyHelper
{
    public static string RegisterEmailConfirmation(string email) => $"auth:register:email-confirmation:{email}";
    public static string UserRefreshToken(Guid userId) => $"auth:user:refresh-token:{userId}";
    public static string UserResetPassword(Guid userId) => $"auth:user:reset-password:{userId}";
}
