using Domain.Aggregates.DestinationAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityFramework.Configurations;

public class DestinationConfiguration : IEntityTypeConfiguration<Destination>
{
    private const int NameMaxLength = 50;
    private const int DescriptionMaxLength = 100;
    
    public void Configure(EntityTypeBuilder<Destination> builder)
    {
        builder.ToTable("Destinations");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .IsRequired();

        builder.Property(d => d.Address)
            .HasMaxLength(NameMaxLength)
            .IsRequired();

        builder.Property(d => d.Description)
            .HasMaxLength(DescriptionMaxLength)
            .IsRequired();

        builder.Property(d => d.Image)
            .HasDefaultValue(string.Empty)
            .IsRequired();

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.Property(d => d.UpdatedAt);
        
        builder.HasMany(d => d.Attractions)
            .WithOne(a => a.Destination)
            .HasForeignKey(a => a.DestinationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.GroupMatches)
            .WithOne(gm => gm.Destination)
            .HasForeignKey(gm => gm.DestinationId);

        builder.HasMany(d => d.GroupMemberVotes)
            .WithOne(gmdv => gmdv.Destination)
            .HasForeignKey(gmdv => gmdv.DestinationId);
    }
}
