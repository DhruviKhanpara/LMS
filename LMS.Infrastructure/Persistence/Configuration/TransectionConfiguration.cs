using LMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configuration;

public class TransectionConfiguration : IEntityTypeConfiguration<Transection>
{
    public void Configure(EntityTypeBuilder<Transection> builder)
    {
        builder.ToTable("Transection");
        builder.HasKey(e => e.Id).HasName("PK_Transection");

        builder.ToTable(nameof(Transection), t => t.HasTrigger("Transection_Audit_Insert"));
        builder.ToTable(nameof(Transection), t => t.HasTrigger("Transection_Audit_Update"));
        builder.ToTable(nameof(Transection), t => t.HasTrigger("Transection_Audit_Delete"));

        builder.ToTable(nameof(Transection), t => t.HasTrigger("Transection_Trigger"));

        builder.Property(b => b.DueDate).HasDefaultValueSql("(getdate())");
        builder.Property(e => e.BorrowDate).HasDefaultValueSql("(getdate())");
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.Property(e => e.RenewCount).HasDefaultValue(0);

        builder.HasOne(d => d.Status).WithMany(p => p.Transections)
            .HasForeignKey(d => d.StatusId)
            .OnDelete(DeleteBehavior.ClientSetNull);
    }
}
