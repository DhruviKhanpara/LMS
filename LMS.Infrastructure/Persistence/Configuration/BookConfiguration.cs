using LMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configuration;

public class BookConfiguration : IEntityTypeConfiguration<Books>
{
    public void Configure(EntityTypeBuilder<Books> builder)
    {
        builder.ToTable("Books");
        builder.HasKey(e => e.Id).HasName("PK_Books");
        builder.HasIndex(e => e.Isbn)
            .HasDatabaseName("UK_Book_ISBN")
            .IsUnique(true)
            .HasFilter("[Isbn] IS NOT NULL");

        builder.ToTable(nameof(Books), t => t.HasTrigger("Books_Audit_Insert"));
        builder.ToTable(nameof(Books), t => t.HasTrigger("Books_Audit_Update"));
        builder.ToTable(nameof(Books), t => t.HasTrigger("Books_Audit_Delete"));

        builder.Property(e => e.Author).HasMaxLength(255);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.Property(e => e.Isbn)
            .HasMaxLength(50)
            .HasColumnName("ISBN");
        builder.Property(e => e.Publisher).HasMaxLength(255);
        builder.Property(e => e.Title).HasMaxLength(255);

        builder.HasOne(d => d.Status).WithMany(p => p.Books)
            .HasForeignKey(d => d.StatusId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.Property(e => e.Price).HasColumnType("decimal(18, 2)");

        builder.HasMany(e => e.BookFileMappings).WithOne(p => p.Books)
            .HasForeignKey(d => d.BookId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.HasMany(e => e.Transections).WithOne(p => p.Book)
            .HasForeignKey(d => d.BookId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.HasMany(e => e.Reservations).WithOne(p => p.Book)
            .HasForeignKey(d => d.BookId)
            .OnDelete(DeleteBehavior.ClientSetNull);
    }
}
