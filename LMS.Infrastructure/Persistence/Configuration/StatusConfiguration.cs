using LMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configuration;

public class StatusConfiguration : IEntityTypeConfiguration<Status>
{
    public void Configure(EntityTypeBuilder<Status> builder)
    {
        builder.ToTable("Status");
        builder.HasKey(e => e.Id).HasName("PK_Status");

        builder.ToTable(nameof(Status), t => t.HasTrigger("Status_Audit_Insert"));
        builder.ToTable(nameof(Status), t => t.HasTrigger("Status_Audit_Update"));
        builder.ToTable(nameof(Status), t => t.HasTrigger("Status_Audit_Delete"));

        builder.Property(e => e.Color)
            .HasMaxLength(50)
            .HasDefaultValue("#ffffff");
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.Property(e => e.Label).HasMaxLength(50);
    }
}
