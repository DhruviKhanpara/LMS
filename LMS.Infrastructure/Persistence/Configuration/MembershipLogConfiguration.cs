using LMS.Core.Entities.LogEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configuration;

public class MembershipLogConfiguration : IEntityTypeConfiguration<MembershipLog>
{
    public void Configure(EntityTypeBuilder<MembershipLog> builder)
    {
        builder.ToTable("MembershipLog", "logging");
        builder.HasKey(e => e.SerialNumber).HasName("PK_MembershipLog");

        builder.Property(e => e.Operation)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.LogType)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.Cost).HasColumnType("decimal(10, 2)");
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.Discount).HasColumnType("decimal(10, 2)");
        builder.Property(e => e.Type).HasMaxLength(255);

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
    }
}
