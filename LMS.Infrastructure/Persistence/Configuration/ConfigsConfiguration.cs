using LMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configuration;

public class ConfigsConfiguration : IEntityTypeConfiguration<Configs>
{
    public void Configure(EntityTypeBuilder<Configs> builder)
    {
        builder.ToTable("Configs");
        builder.HasKey(e => e.Id).HasName("PK_Configs");

        builder.ToTable(nameof(Configs), t => t.HasTrigger("Configs_Audit_Insert"));
        builder.ToTable(nameof(Configs), t => t.HasTrigger("Configs_Audit_Update"));
        builder.ToTable(nameof(Configs), t => t.HasTrigger("Configs_Audit_Delete"));

        builder.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.Property(e => e.KeyName).HasMaxLength(50);
        builder.Property(e => e.KeyValue).HasMaxLength(1000);
    }
}
