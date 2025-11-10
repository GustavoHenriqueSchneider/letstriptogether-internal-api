using Domain.Aggregates.GroupAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityFramework.Configurations;

public class GroupInvitationConfiguration : IEntityTypeConfiguration<GroupInvitation>
{
    public void Configure(EntityTypeBuilder<GroupInvitation> builder)
    {
        builder.ToTable("GroupInvitations");

        builder.HasKey(gi => gi.Id);

        builder.Property(gi => gi.Id)
            .IsRequired();

        builder.Property(gi => gi.GroupId)
            .IsRequired();

        builder.Property(gi => gi.ExpirationDate)
            .IsRequired();

        builder.Property(gi => gi.Status)
            .IsRequired();

        builder.Property(gi => gi.CreatedAt)
            .IsRequired();

        builder.Property(gi => gi.UpdatedAt);
        
        builder.HasOne(gi => gi.Group)
            .WithMany(g => g.Invitations)
            .HasForeignKey(gi => gi.GroupId);

        builder.HasMany(gi => gi.AnsweredBy)
            .WithOne(ugi => ugi.GroupInvitation)
            .HasForeignKey(ugi => ugi.GroupInvitationId);
    }
}
