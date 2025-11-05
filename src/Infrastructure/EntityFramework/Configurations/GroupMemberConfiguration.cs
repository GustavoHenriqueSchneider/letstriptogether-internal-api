using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetsTripTogether.InternalApi.Infrastructure.EntityFramework.Configurations;

public class GroupMemberConfiguration : IEntityTypeConfiguration<GroupMember>
{
    public void Configure(EntityTypeBuilder<GroupMember> builder)
    {
        builder.ToTable("GroupMembers");

        builder.HasKey(gm => gm.Id);

        builder.Property(gm => gm.Id)
            .IsRequired();

        builder.Property(gm => gm.GroupId)
            .IsRequired();

        builder.Property(gm => gm.UserId)
            .IsRequired();

        builder.Property(gm => gm.IsOwner)
            .IsRequired();

        builder.Property(gm => gm.CreatedAt)
            .IsRequired();

        builder.Property(gm => gm.UpdatedAt);
        
        builder.HasOne(gm => gm.Group)
            .WithMany(g => g.Members)
            .HasForeignKey(gm => gm.GroupId);

        builder.HasOne(gm => gm.User)
            .WithMany(u => u.GroupMemberships)
            .HasForeignKey(gm => gm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(gm => gm.Votes)
            .WithOne(gmdv => gmdv.GroupMember)
            .HasForeignKey(gmdv => gmdv.GroupMemberId);
    }
}
