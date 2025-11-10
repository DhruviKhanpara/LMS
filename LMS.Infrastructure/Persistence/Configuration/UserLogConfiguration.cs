using LMS.Core.Entities.LogEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configuration;

public class UserLogConfiguration : IEntityTypeConfiguration<UserLog>
{
    public void Configure(EntityTypeBuilder<UserLog> builder)
    {
        builder.ToTable("UserLog", "logging");
        builder.HasKey(e => e.SerialNumber).HasName("PK_UserLog");

        builder.Property(e => e.Operation)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.LogType)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.Address).HasMaxLength(255);
        builder.Property(e => e.Dob).HasColumnName("DOB");
        builder.Property(e => e.Email).HasMaxLength(255);
        builder.Property(e => e.FirstName)
            .HasMaxLength(100)
            .IsUnicode(false);
        builder.Property(e => e.Gender)
            .HasMaxLength(50)
            .IsUnicode(false);
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.Property(e => e.LastName)
            .HasMaxLength(100)
            .IsUnicode(false);
        builder.Property(e => e.LibraryCardNumber).HasMaxLength(50);
        builder.Property(e => e.MiddleName)
            .HasMaxLength(100)
            .IsUnicode(false);
        builder.Property(e => e.MobileNo)
            .HasMaxLength(12)
            .IsUnicode(false);
        builder.Property(e => e.Username).HasMaxLength(50);

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
