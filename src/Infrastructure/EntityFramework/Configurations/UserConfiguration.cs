using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace LetsTripTogether.InternalApi.Infrastructure.EntityFramework.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    private const int BcryptPasswordHashMaxLength = 60;
    
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);
        
        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.Property(u => u.Id)
            .IsRequired();

        builder.Property(u => u.Name)
            .HasMaxLength(User.NameMaxLength)
            .IsRequired();

        builder.Property(u => u.Email)
            .HasMaxLength(User.EmailMaxLength)
            .IsRequired();

        builder.Property(u => u.PasswordHash)
            .HasMaxLength(BcryptPasswordHashMaxLength)
            .IsRequired();

        builder.Property(u => u.IsAnonymous)
            .IsRequired();

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.UpdatedAt);
        
        builder.HasOne(u => u.Preferences)
            .WithOne(up => up.User)
            .HasForeignKey<UserPreference>(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.GroupMemberships)
            .WithOne(gm => gm.User)
            .HasForeignKey(gm => gm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.AcceptedInvitations)
            .WithOne(ugi => ugi.User)
            .HasForeignKey(ugi => ugi.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
