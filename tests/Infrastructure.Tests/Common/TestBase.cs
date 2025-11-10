using Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tests.Common;

public abstract class TestBase : IDisposable
{
    protected AppDbContext DbContext { get; private set; } = null!;
    private readonly string _connectionString;

    protected TestBase()
    {
        _connectionString = "Host=localhost;Port=5432;Database=letstriptogether_test;Username=admin;Password=root";
        
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_connectionString)
            .Options;

        DbContext = new AppDbContext(options);
        EnsureDatabaseCreated();
    }

    private void EnsureDatabaseCreated()
    {
        try
        {
            DbContext.Database.EnsureCreated();
        }
        catch
        {
            // Database might already exist
        }
    }

    protected async Task CleanDatabaseAsync()
    {
        var tables = new[]
        {
            "GroupMemberDestinationVotes",
            "GroupMembers",
            "GroupPreferences",
            "GroupMatches",
            "GroupInvitations",
            "Groups",
            "UserGroupInvitations",
            "UserPreferences",
            "UserRoles",
            "Users",
            "Roles",
            "DestinationAttractions",
            "Destinations"
        };

        foreach (var table in tables)
        {
            try
            {
                await DbContext.Database.ExecuteSqlAsync($"TRUNCATE TABLE \"{table}\" CASCADE");
            }
            catch
            {
                // Ignore errors
            }
        }
    }

    public void Dispose()
    {
        DbContext?.Dispose();
    }
}
