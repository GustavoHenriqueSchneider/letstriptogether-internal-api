using Domain.Aggregates.UserAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityFramework.Configurations;

public class UserPreferenceConfiguration : IEntityTypeConfiguration<UserPreference>
{
    public void Configure(EntityTypeBuilder<UserPreference> builder)
    {
        builder.ToTable("UserPreferences");

        builder.HasKey(up => up.Id);

        builder.Property(up => up.Id)
            .IsRequired();

        builder.Property(up => up.UserId)
            .IsRequired();

        builder.Property(up => up.LikesShopping)
            .IsRequired();
        
        builder.Property(gp => gp.LikesGastronomy)
            .IsRequired();

        builder.Property(up => up.CreatedAt)
            .IsRequired();

        builder.Property(up => up.UpdatedAt);

        builder.Property<List<string>>("_culture")
            .HasColumnName(nameof(UserPreference.Culture))
            .IsRequired();

        builder.Property<List<string>>("_entertainment")
            .HasColumnName(nameof(UserPreference.Entertainment))
            .IsRequired();

        builder.Property<List<string>>("_placeTypes")
            .HasColumnName(nameof(UserPreference.PlaceTypes))
            .IsRequired();
        
        builder.HasOne(up => up.User)
            .WithOne(u => u.Preferences)
            .HasForeignKey<UserPreference>(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
