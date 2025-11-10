using LMS.Core.Entities.LogEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configuration;

public class BooksLogConfiguration : IEntityTypeConfiguration<BooksLog>
{
    public void Configure(EntityTypeBuilder<BooksLog> builder)
    {
        builder.ToTable("BooksLog", "logging");
        builder.HasKey(e => e.SerialNumber).HasName("PK_BooksLog");

        builder.Property(e => e.Operation)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.LogType)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.Author).HasMaxLength(255);
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.Property(e => e.Isbn)
            .HasMaxLength(50)
            .HasColumnName("ISBN");

        builder.Property(e => e.Publisher).HasMaxLength(255);
        builder.Property(e => e.Title).HasMaxLength(255);

        builder.Property(e => e.Price).HasColumnType("decimal(18, 2)");

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
    }
}
