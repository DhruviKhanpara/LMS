using LMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configuration;

public class OutboxMessagesConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");
        builder.HasKey(e => e.Id).HasName("PK_OutboxMessages");

        builder.ToTable("OutboxMessages", t => t.HasTrigger("OutboxMessages_Audit_Insert"));
        builder.ToTable("OutboxMessages", t => t.HasTrigger("OutboxMessages_Audit_Update"));
        builder.ToTable("OutboxMessages", t => t.HasTrigger("OutboxMessages_Audit_Delete"));

        builder.Property(e => e.Type).HasMaxLength(50);
        builder.Property(e => e.IsActive).HasDefaultValue(true);
    }
}
