using LMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configuration;

public class BookFileMappingConfiguration : IEntityTypeConfiguration<BookFileMapping>
{
    public void Configure(EntityTypeBuilder<BookFileMapping> builder)
    {
        builder.ToTable("BookFileMapping");
        builder.HasKey(e => e.Id).HasName("PK_BookFileMapping");

        builder.ToTable(nameof(BookFileMapping), t => t.HasTrigger("BookFileMapping_Audit_Insert"));
        builder.ToTable(nameof(BookFileMapping), t => t.HasTrigger("BookFileMapping_Audit_Update"));
        builder.ToTable(nameof(BookFileMapping), t => t.HasTrigger("BookFileMapping_Audit_Delete"));

        builder.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        builder.Property(e => e.IsActive).HasDefaultValue(true);

        builder.Property(e => e.Label).HasMaxLength(50);
    }
}
