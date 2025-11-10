using LMS.Core.Entities.LogEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configuration;

public class OutboxMessagesLogConfiguration : IEntityTypeConfiguration<OutboxMessagesLog>
{
    public void Configure(EntityTypeBuilder<OutboxMessagesLog> builder)
    {
        builder.ToTable("OutboxMessagesLog", "logging");
        builder.HasKey(e => e.SerialNumber).HasName("PK_OutboxMessagesLog");

        builder.Property(e => e.Operation)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.LogType)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.Type).HasMaxLength(50);
    }
}
