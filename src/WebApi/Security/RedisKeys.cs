namespace WebApi.Security;

public class RedisKeys
{
    public const string RegisterEmailConfirmation = "auth:register:email-confirmation:{email}";
    public const string UserRefreshToken = "auth:user:refresh-token:{userId}";
}
