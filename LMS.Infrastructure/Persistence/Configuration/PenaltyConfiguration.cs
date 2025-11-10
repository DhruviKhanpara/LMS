using LMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configuration;

public class PenaltyConfiguration : IEntityTypeConfiguration<Penalty>
{
    public void Configure(EntityTypeBuilder<Penalty> builder)
    {
        builder.ToTable("Penalty");
        builder.HasKey(e => e.Id).HasName("PK_Penalty");

        builder.ToTable(nameof(Penalty), t => t.HasTrigger("Penalty_Audit_Insert"));
        builder.ToTable(nameof(Penalty), t => t.HasTrigger("Penalty_Audit_Update"));
        builder.ToTable(nameof(Penalty), t => t.HasTrigger("Penalty_Audit_Delete"));

        builder.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.IsActive).HasDefaultValue(true);

        builder.HasOne(d => d.Status).WithMany(p => p.Penalties)
            .HasForeignKey(d => d.StatusId)
            .OnDelete(DeleteBehavior.ClientSetNull);
    }
}
