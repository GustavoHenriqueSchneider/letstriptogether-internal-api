using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetsTripTogether.InternalApi.Infrastructure.EntityFramework.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ToTable("Groups");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Id)
            .IsRequired();

        builder.Property(g => g.Name)
            .HasMaxLength(Group.NameMaxLength)
            .IsRequired();

        builder.Property(g => g.TripExpectedDate)
            .IsRequired();

        builder.Property(g => g.CreatedAt)
            .IsRequired();

        builder.Property(g => g.UpdatedAt);
        
        builder.HasMany(g => g.Invitations)
            .WithOne(gi => gi.Group)
            .HasForeignKey(gi => gi.GroupId);

        builder.HasMany(g => g.Matches)
            .WithOne(gm => gm.Group)
            .HasForeignKey(gm => gm.GroupId);

        builder.HasMany(g => g.Members)
            .WithOne(gm => gm.Group)
            .HasForeignKey(gm => gm.GroupId);

        builder.HasOne(g => g.Preferences)
            .WithOne(gp => gp.Group)
            .HasForeignKey<GroupPreference>(gp => gp.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
