using LMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configuration;

public class MembershipConfiguration : IEntityTypeConfiguration<Membership>
{
    public void Configure(EntityTypeBuilder<Membership> builder)
    {
        builder.ToTable("Membership");
        builder.HasKey(e => e.Id).HasName("PK_Membership");

        builder.ToTable(nameof(Membership), t => t.HasTrigger("Membership_Audit_Insert"));
        builder.ToTable(nameof(Membership), t => t.HasTrigger("Membership_Audit_Update"));
        builder.ToTable(nameof(Membership), t => t.HasTrigger("Membership_Audit_Delete"));

        builder.Property(e => e.Cost).HasColumnType("decimal(10, 2)");
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.Discount).HasColumnType("decimal(10, 2)");
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.Property(e => e.Type).HasMaxLength(255);
    }
}
