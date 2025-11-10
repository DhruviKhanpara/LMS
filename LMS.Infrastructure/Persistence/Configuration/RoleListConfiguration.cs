using LMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configuration;

public class RoleListConfiguration : IEntityTypeConfiguration<RoleList>
{
    public void Configure(EntityTypeBuilder<RoleList> builder)
    {
        builder.ToTable("RoleList");
        builder.HasKey(e => e.Id).HasName("PK_RoleList");

        builder.ToTable(nameof(RoleList), t => t.HasTrigger("RoleList_Audit_Insert"));
        builder.ToTable(nameof(RoleList), t => t.HasTrigger("RoleList_Audit_Update"));
        builder.ToTable(nameof(RoleList), t => t.HasTrigger("RoleList_Audit_Delete"));

        builder.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.Property(e => e.Label).HasMaxLength(100);
        builder.Property(e => e.Description).HasMaxLength(500);
    }
}
