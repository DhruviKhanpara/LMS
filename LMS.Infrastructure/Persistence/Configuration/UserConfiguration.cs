using LMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User");
        builder.HasKey(e => e.Id).HasName("PK_User");

        builder.ToTable(nameof(User), t => t.HasTrigger("User_Audit_Insert"));
        builder.ToTable(nameof(User), t => t.HasTrigger("User_Audit_Update"));
        builder.ToTable(nameof(User), t => t.HasTrigger("User_Audit_Delete"));

        builder.HasIndex(e => new { e.Username, e.Email, e.RoleId })
            .HasDatabaseName("UK_User_UsernameEmailRole")
            .IsUnique(true);

        builder.HasIndex(e => e.LibraryCardNumber)
            .HasDatabaseName("UK_User_LibraryCardNumber")
            .HasFilter("[LibraryCardNumber] IS NOT NULL");

        builder.Property(e => e.Address).HasMaxLength(255);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        builder.Property(e => e.Dob).HasColumnName("DOB");
        builder.Property(e => e.Email).HasMaxLength(255);
        builder.Property(e => e.FirstName)
            .HasMaxLength(100)
            .IsUnicode(false);
        builder.Property(e => e.Gender)
            .HasMaxLength(50)
            .IsUnicode(false);
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.Property(e => e.JoiningDate).HasDefaultValueSql("(getdate())");
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
    }
}
