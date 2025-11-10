using LMS.Core.Entities.LogEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configuration;

public class BookFileMappingLogConfiguration : IEntityTypeConfiguration<BookFileMappingLog>
{
    public void Configure(EntityTypeBuilder<BookFileMappingLog> builder)
    {
        builder.ToTable("BookFileMappingLog", "logging");
        builder.HasKey(e => e.SerialNumber).HasName("PK_BookFileMappingLog");

        builder.Property(e => e.Operation)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.LogType)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.Label).HasMaxLength(50);
    }
}
