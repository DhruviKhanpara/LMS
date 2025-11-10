using LMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configuration;

public class UserMembershipMappingConfiguration : IEntityTypeConfiguration<UserMembershipMapping>
{
    public void Configure(EntityTypeBuilder<UserMembershipMapping> builder)
    {
        builder.ToTable("UserMembershipMapping");
        builder.HasKey(e => e.Id).HasName("PK_UserMembershipMapping");

        builder.ToTable(nameof(UserMembershipMapping), t => t.HasTrigger("UserMembershipMapping_Audit_Insert"));
        builder.ToTable(nameof(UserMembershipMapping), t => t.HasTrigger("UserMembershipMapping_Audit_Update"));
        builder.ToTable(nameof(UserMembershipMapping), t => t.HasTrigger("UserMembershipMapping_Audit_Delete"));

        builder.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        builder.Property(e => e.Discount).HasColumnType("decimal(10, 2)");
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.Property(e => e.MembershipCost).HasColumnType("decimal(10, 2)");
        builder.Property(e => e.PaidAmount)
            .HasComputedColumnSql("([MembershipCost]-[Discount])", true)
            .HasColumnType("decimal(11, 2)");
    }
}
