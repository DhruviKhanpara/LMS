using LMS.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Persistence.Configuration;

public class StatusTypeConfiguration : IEntityTypeConfiguration<StatusType>
{
    public void Configure(EntityTypeBuilder<StatusType> builder)
    {
        builder.ToTable("StatusType");
        builder.HasKey(e => e.Id).HasName("PK_StatusType");

        builder.ToTable(nameof(StatusType), t => t.HasTrigger("StatusType_Audit_Insert"));
        builder.ToTable(nameof(StatusType), t => t.HasTrigger("StatusType_Audit_Update"));
        builder.ToTable(nameof(StatusType), t => t.HasTrigger("StatusType_Audit_Delete"));

        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.Property(e => e.Label).HasMaxLength(50);
    }
}
