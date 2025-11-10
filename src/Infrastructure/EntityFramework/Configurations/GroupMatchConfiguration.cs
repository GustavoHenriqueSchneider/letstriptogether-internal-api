using Domain.Aggregates.GroupAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityFramework.Configurations;

public class GroupMatchConfiguration : IEntityTypeConfiguration<GroupMatch>
{
    public void Configure(EntityTypeBuilder<GroupMatch> builder)
    {
        builder.ToTable("GroupMatches");

        builder.HasKey(gm => gm.Id);

        builder.Property(gm => gm.Id)
            .IsRequired();

        builder.Property(gm => gm.GroupId)
            .IsRequired();

        builder.Property(gm => gm.DestinationId)
            .IsRequired();

        builder.Property(gm => gm.CreatedAt)
            .IsRequired();

        builder.Property(gm => gm.UpdatedAt);
        
        builder.HasOne(gm => gm.Group)
            .WithMany(g => g.Matches)
            .HasForeignKey(gm => gm.GroupId);

        builder.HasOne(gm => gm.Destination)
            .WithMany(d => d.GroupMatches)
            .HasForeignKey(gm => gm.DestinationId);
    }
}
