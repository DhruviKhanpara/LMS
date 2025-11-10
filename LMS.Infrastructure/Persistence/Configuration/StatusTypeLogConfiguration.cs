using LMS.Core.Entities.LogEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configuration;

public class StatusTypeLogConfiguration : IEntityTypeConfiguration<StatusTypeLog>
{
    public void Configure(EntityTypeBuilder<StatusTypeLog> builder)
    {
        builder.ToTable("StatusTypeLog", "logging");
        builder.HasKey(e => e.SerialNumber).HasName("PK_StatusTypeLog");

        builder.Property(e => e.Operation)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.LogType)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.Label).HasMaxLength(50);
    }
}
