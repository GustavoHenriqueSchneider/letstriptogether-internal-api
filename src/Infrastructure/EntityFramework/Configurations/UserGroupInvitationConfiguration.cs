using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetsTripTogether.InternalApi.Infrastructure.EntityFramework.Configurations;

public class UserGroupInvitationConfiguration : IEntityTypeConfiguration<UserGroupInvitation>
{
    public void Configure(EntityTypeBuilder<UserGroupInvitation> builder)
    {
        builder.ToTable("UserGroupInvitations");

        builder.HasKey(ugi => ugi.Id);

        builder.Property(ugi => ugi.Id)
            .IsRequired();

        builder.Property(ugi => ugi.GroupInvitationId)
            .IsRequired();

        builder.Property(ugi => ugi.UserId)
            .IsRequired();

        builder.Property(ugi => ugi.IsAccepted)
            .IsRequired();

        builder.Property(ugi => ugi.CreatedAt)
            .IsRequired();

        builder.Property(ugi => ugi.UpdatedAt);
        
        builder.HasOne(ugi => ugi.GroupInvitation)
            .WithMany(gi => gi.AnsweredBy)
            .HasForeignKey(ugi => ugi.GroupInvitationId);

        builder.HasOne(ugi => ugi.User)
            .WithMany(u => u.AcceptedInvitations)
            .HasForeignKey(ugi => ugi.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
