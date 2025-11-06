namespace Infrastructure.UnitTests.Common;

public static class TestDataHelper
{
    public static string GenerateRandomEmail()
    {
        return $"test_{Guid.NewGuid():N}@example.com";
    }

    public static string GenerateRandomName()
    {
        return $"Test User {Guid.NewGuid():N}";
    }

    public static string GenerateValidPassword()
    {
        return $"ValidPass{Guid.NewGuid():N}!123";
    }

    public static Guid GenerateRandomGuid()
    {
        return Guid.NewGuid();
    }
}
