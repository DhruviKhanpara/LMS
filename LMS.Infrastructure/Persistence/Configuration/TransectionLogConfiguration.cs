using LMS.Core.Entities.LogEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configuration;

public class TransectionLogConfiguration : IEntityTypeConfiguration<TransectionLog>
{
    public void Configure(EntityTypeBuilder<TransectionLog> builder)
    {
        builder.ToTable("TransectionLog", "logging");
        builder.HasKey(e => e.SerialNumber).HasName("PK_TransectionLog");

        builder.Property(e => e.Operation)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.LogType)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.HasOne(e => e.CreatedByUser)
              .WithMany()
              .HasForeignKey(e => e.CreatedBy)
              .IsRequired(false)
              .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ModifiedByUser)
              .WithMany()
              .HasForeignKey(e => e.ModifiedBy)
              .IsRequired(false)
              .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.DeletedByUser)
              .WithMany()
              .HasForeignKey(e => e.DeletedBy)
              .IsRequired(false)
              .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.User)
              .WithMany()
              .HasForeignKey(e => e.UserId)
              .IsRequired(false)
              .OnDelete(DeleteBehavior.Restrict);
    }
}
