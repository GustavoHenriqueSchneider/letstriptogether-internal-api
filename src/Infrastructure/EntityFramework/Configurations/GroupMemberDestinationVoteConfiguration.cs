using Domain.Aggregates.GroupAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityFramework.Configurations;

public class GroupMemberDestinationVoteConfiguration : IEntityTypeConfiguration<GroupMemberDestinationVote>
{
    public void Configure(EntityTypeBuilder<GroupMemberDestinationVote> builder)
    {
        builder.ToTable("GroupMemberDestinationVotes");

        builder.HasKey(gmdv => gmdv.Id);

        builder.Property(gmdv => gmdv.Id)
            .IsRequired();

        builder.Property(gmdv => gmdv.GroupMemberId)
            .IsRequired();

        builder.Property(gmdv => gmdv.DestinationId)
            .IsRequired();

        builder.Property(gmdv => gmdv.IsApproved)
            .IsRequired();

        builder.Property(gmdv => gmdv.CreatedAt)
            .IsRequired();

        builder.Property(gmdv => gmdv.UpdatedAt);
        
        builder.HasOne(gmdv => gmdv.GroupMember)
            .WithMany(gm => gm.Votes)
            .HasForeignKey(gmdv => gmdv.GroupMemberId);

        builder.HasOne(gmdv => gmdv.Destination)
            .WithMany(d => d.GroupMemberVotes)
            .HasForeignKey(gmdv => gmdv.DestinationId);
    }
}
