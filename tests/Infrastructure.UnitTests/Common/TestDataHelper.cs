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

    public static string GenerateRandomGroupName()
    {
        // Group.NameMaxLength = 30, so we need to keep it short
        // "G" + GUID (32 chars) = 33 chars, too long
        // Use first 28 chars of GUID to stay within limit
        var guidPart = Guid.NewGuid().ToString("N")[..28];
        return $"G{guidPart}";
    }
}
