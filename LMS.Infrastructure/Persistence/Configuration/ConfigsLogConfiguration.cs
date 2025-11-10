using LMS.Core.Entities.LogEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configuration;

public class ConfigsLogConfiguration : IEntityTypeConfiguration<ConfigsLog>
{
    public void Configure(EntityTypeBuilder<ConfigsLog> builder)
    {
        builder.ToTable("ConfigsLog", "logging");
        builder.HasKey(e => e.SerialNumber).HasName("PK_ConfigsLog");

        builder.Property(e => e.Operation)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.LogType)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.KeyName).HasMaxLength(50);
        builder.Property(e => e.KeyValue).HasMaxLength(1000);

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
