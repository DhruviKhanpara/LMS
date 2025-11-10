using LMS.Core.Entities.LogEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configuration;

public class UserMembershipMappingLogConfiguration : IEntityTypeConfiguration<UserMembershipMappingLog>
{
    public void Configure(EntityTypeBuilder<UserMembershipMappingLog> builder)
    {
        builder.ToTable("UserMembershipMappingLog", "logging");
        builder.HasKey(e => e.SerialNumber).HasName("PK_UserMembershipMappingLog");

        builder.Property(e => e.Operation)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.LogType)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.Discount).HasColumnType("decimal(10, 2)");
        builder.Property(e => e.MembershipCost).HasColumnType("decimal(10, 2)");
        builder.Property(e => e.PaidAmount).HasColumnType("decimal(11, 2)");

        builder.HasOne(e => e.CreatedByUser)
              .WithMany()
              .HasForeignKey(e => e.CreatedBy)
              .IsRequired(false)
              .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ModifiedByUser)
              .WithMany()
              .HasForeignKey(e => e.ModifiedBy)
              .IsRequired(false)
              .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.DeletedByUser)
              .WithMany()
              .HasForeignKey(e => e.DeletedBy)
              .IsRequired(false)
              .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.User)
              .WithMany()
              .HasForeignKey(e => e.UserId)
              .IsRequired(false)
              .OnDelete(DeleteBehavior.Restrict);
    }
}
