using Domain.Aggregates.DestinationAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityFramework.Configurations;

public class DestinationAttractionConfiguration : IEntityTypeConfiguration<DestinationAttraction>
{
    private const int NameMaxLength = 70;
    private const int DescriptionMaxLength = 120;
    private const int CategoryMaxLength = 35;
    
    public void Configure(EntityTypeBuilder<DestinationAttraction> builder)
    {
        builder.ToTable("DestinationAttractions");

        builder.HasKey(da => da.Id);

        builder.Property(da => da.Id)
            .IsRequired();

        builder.Property(da => da.DestinationId)
            .IsRequired();

        builder.Property(da => da.Name)
            .HasMaxLength(NameMaxLength)
            .IsRequired();

        builder.Property(da => da.Description)
            .HasMaxLength(DescriptionMaxLength)
            .IsRequired();

        builder.Property(da => da.Category)
            .HasMaxLength(CategoryMaxLength)
            .IsRequired();

        builder.Property(da => da.CreatedAt)
            .IsRequired();

        builder.Property(da => da.UpdatedAt);
        
        builder.HasOne(da => da.Destination)
            .WithMany(d => d.Attractions)
            .HasForeignKey(da => da.DestinationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
