using Domain.Aggregates.GroupAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityFramework.Configurations;

public class GroupPreferenceConfiguration : IEntityTypeConfiguration<GroupPreference>
{
    public void Configure(EntityTypeBuilder<GroupPreference> builder)
    {
        builder.ToTable("GroupPreferences");

        builder.HasKey(gp => gp.Id);

        builder.Property(gp => gp.Id)
            .IsRequired();

        builder.Property(gp => gp.GroupId)
            .IsRequired();

        builder.Property(gp => gp.LikesCommercial)
            .IsRequired();

        builder.Property(gp => gp.CreatedAt)
            .IsRequired();

        builder.Property(gp => gp.UpdatedAt);
        
        builder.Property<List<string>>("_food")
            .HasColumnName(nameof(GroupPreference.Food))
            .IsRequired();

        builder.Property<List<string>>("_culture")
            .HasColumnName(nameof(GroupPreference.Culture))
            .IsRequired();

        builder.Property<List<string>>("_entertainment")
            .HasColumnName(nameof(GroupPreference.Entertainment))
            .IsRequired();

        builder.Property<List<string>>("_placeTypes")
            .HasColumnName(nameof(GroupPreference.PlaceTypes))
            .IsRequired();
        
        builder.HasOne(gp => gp.Group)
            .WithOne(g => g.Preferences)
            .HasForeignKey<GroupPreference>(gp => gp.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
