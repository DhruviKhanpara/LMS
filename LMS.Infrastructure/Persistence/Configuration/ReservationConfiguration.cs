using LMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configuration;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("Reservation");
        builder.HasKey(e => e.Id).HasName("PK_Reservation");

        builder.ToTable(nameof(Reservation), t => t.HasTrigger("Reservation_Audit_Insert"));
        builder.ToTable(nameof(Reservation), t => t.HasTrigger("Reservation_Audit_Update"));
        builder.ToTable(nameof(Reservation), t => t.HasTrigger("Reservation_Audit_Delete"));

        builder.Property(e => e.ReservationDate).HasDefaultValueSql("(getdate())");
        builder.Property(e => e.AllocateAfter).HasDefaultValueSql("(getdate())");
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.Property(e => e.IsAllocated).HasDefaultValue(false);
        builder.Property(e => e.TransferAllocationCount).HasDefaultValue(0);

        builder.Property(e => e.CancelReason)
            .HasMaxLength(255)
            .IsUnicode(false);

        builder.HasOne(d => d.Status).WithMany(p => p.Reservations)
            .HasForeignKey(d => d.StatusId)
            .OnDelete(DeleteBehavior.ClientSetNull);
    }
}
