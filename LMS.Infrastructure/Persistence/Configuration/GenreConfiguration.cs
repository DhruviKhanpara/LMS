using LMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configuration;

public class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.ToTable("Genre");
        builder.HasKey(e => e.Id).HasName("PK_Genre");

        builder.ToTable(nameof(Genre), t => t.HasTrigger("Genre_Audit_Insert"));
        builder.ToTable(nameof(Genre), t => t.HasTrigger("Genre_Audit_Update"));
        builder.ToTable(nameof(Genre), t => t.HasTrigger("Genre_Audit_Delete"));

        builder.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.Name).HasMaxLength(100);
        builder.Property(e => e.IsActive).HasDefaultValue(true);
    }
}
