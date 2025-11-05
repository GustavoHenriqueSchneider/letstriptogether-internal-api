using LetsTripTogether.InternalApi.Domain.Aggregates.DestinationAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace LetsTripTogether.InternalApi.Infrastructure.EntityFramework.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<Destination> Destinations { get; init; }
    public DbSet<DestinationAttraction> DestinationAttractions { get; init; }
    public DbSet<Group> Groups { get; init; }
    public DbSet<GroupInvitation> GroupInvitations { get; init; }
    public DbSet<GroupMatch> GroupMatches { get; init; }
    public DbSet<GroupMember> GroupMembers { get; init; }
    public DbSet<GroupMemberDestinationVote> GroupMemberDestinationVotes { get; init; }
    public DbSet<GroupPreference> GroupPreferences { get; init; }
    public DbSet<Role> Roles { get; init; }
    public DbSet<User> Users { get; init; }
    public DbSet<UserGroupInvitation> UserGroupInvitations { get; init; }
    public DbSet<UserPreference> UserPreferences { get; init; }
    public DbSet<UserRole> UserRoles { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
    
    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        await base.SaveChangesAsync(cancellationToken);
    }
}
