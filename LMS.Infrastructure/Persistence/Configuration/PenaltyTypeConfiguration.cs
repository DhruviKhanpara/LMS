using LMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configuration;

public class PenaltyTypeConfiguration : IEntityTypeConfiguration<PenaltyType>
{
    public void Configure(EntityTypeBuilder<PenaltyType> builder)
    {
        builder.ToTable("PenaltyType");
        builder.HasKey(e => e.Id).HasName("PK_PenaltyType");

        builder.ToTable(nameof(PenaltyType), t => t.HasTrigger("PenaltyType_Audit_Insert"));
        builder.ToTable(nameof(PenaltyType), t => t.HasTrigger("PenaltyType_Audit_Update"));
        builder.ToTable(nameof(PenaltyType), t => t.HasTrigger("PenaltyType_Audit_Delete"));

        builder.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.Property(e => e.Label).HasMaxLength(50);
    }
}
