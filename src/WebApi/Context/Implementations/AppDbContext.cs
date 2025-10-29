using Microsoft.EntityFrameworkCore;
using WebApi.Models;
namespace WebApi.Context.Implementations;

// TODO: aplicar clean arc
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Destination> Destinations { get; init; }
    public DbSet<Group> Groups { get; init; }
    public DbSet<GroupInvitation> GroupInvitations { get; init; }
    public DbSet<GroupMatch> GroupMatches { get; init; }
    public DbSet<GroupMember> GroupMembers { get; init; }
    public DbSet<GroupMemberDestinationVote> GroupMemberDestinationVotes { get; init; }
    public DbSet<Role> Roles { get; init; }
    public DbSet<User> Users { get; init; }
    public DbSet<UserGroupInvitation> UserGroupInvitations { get; init; }
    public DbSet<UserPreference> UserPreferences { get; init; }
    public DbSet<UserRole> UserRoles { get; init; }

    // TODO: configurações precisam ir para configurations de cada model
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<GroupMatch>()
            .HasOne(gm => gm.Group)
            .WithMany(g => g.Matches)
            .HasForeignKey(gm => gm.GroupId);

        modelBuilder.Entity<GroupMatch>()
            .HasOne(gm => gm.Destination)
            .WithMany(d => d.GroupMatches)
            .HasForeignKey(gm => gm.DestinationId);

        modelBuilder.Entity<GroupMember>()
            .HasOne(gm => gm.Group)
            .WithMany(g => g.Members)
            .HasForeignKey(gm => gm.GroupId);

        modelBuilder.Entity<GroupMember>()
            .HasOne(gm => gm.User)
            .WithMany(u => u.GroupMemberships)
            .HasForeignKey(gm => gm.UserId);

        modelBuilder.Entity<GroupMemberDestinationVote>()
            .HasOne(gmdv => gmdv.GroupMember)
            .WithMany(gm => gm.Votes)
            .HasForeignKey(gmdv => gmdv.GroupMemberId);

        modelBuilder.Entity<GroupMemberDestinationVote>()
            .HasOne(gmdv => gmdv.Destination)
            .WithMany(d => d.GroupMemberVotes)
            .HasForeignKey(gmdv => gmdv.DestinationId);

        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Preferences)
            .WithOne(up => up.User)
            .HasForeignKey<UserPreference>(up => up.UserId);

        modelBuilder.Entity<UserGroupInvitation>()
            .HasOne(ugi => ugi.GroupInvitation)
            .WithMany(d => d.AnsweredBy)
            .HasForeignKey(ugi => ugi.GroupInvitationId);

        modelBuilder.Entity<UserGroupInvitation>()
            .HasOne(ugi => ugi.User)
            .WithMany(u => u.AcceptedInvitations)
            .HasForeignKey(ugi => ugi.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GroupMember>()
            .HasOne(gm => gm.User)
            .WithMany(u => u.GroupMemberships)
            .HasForeignKey(gm => gm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserPreference>()
            .HasOne(up => up.User)
            .WithOne(u => u.Preferences)
            .HasForeignKey<UserPreference>(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserPreference>()
            .Property<List<string>>("_categories")
            .HasColumnName("Categories");
    }
}
