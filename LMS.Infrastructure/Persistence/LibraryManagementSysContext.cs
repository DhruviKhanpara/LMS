using LMS.Core.Entities;
using LMS.Core.Entities.LogEntities;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Persistence;

public partial class LibraryManagementSysContext : DbContext
{
    public LibraryManagementSysContext()
    {
    }

    public LibraryManagementSysContext(DbContextOptions<LibraryManagementSysContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Books> Books { get; set; }
    public virtual DbSet<Configs> Configs { get; set; }
    public virtual DbSet<Genre> Genre { get; set; }
    public virtual DbSet<Penalty> Penalty { get; set; }
    public virtual DbSet<Reservation> Reservation { get; set; }
    public virtual DbSet<Status> Status { get; set; }
    public virtual DbSet<Transection> Transection { get; set; }
    public virtual DbSet<User> User { get; set; }
    public virtual DbSet<RoleList> RoleList { get; set; }
    public virtual DbSet<Membership> Membership { get; set; }
    public virtual DbSet<UserMembershipMapping> UserMembershipMapping { get; set; }
    public virtual DbSet<StatusType> StatusType { get; set; }
    public virtual DbSet<BookFileMapping> BookFileMapping { get; set; }
    public virtual DbSet<PenaltyType> PenaltyType { get; set; }
    public virtual DbSet<OutboxMessage> OutboxMessages { get; set; }

    public virtual DbSet<BooksLog> BooksLog { get; set; }
    public virtual DbSet<ConfigsLog> ConfigsLog { get; set; }
    public virtual DbSet<GenreLog> GenreLog { get; set; }
    public virtual DbSet<PenaltyLog> PenaltyLog { get; set; }
    public virtual DbSet<ReservationLog> ReservationLog { get; set; }
    public virtual DbSet<StatusLog> StatusLog { get; set; }
    public virtual DbSet<TransectionLog> TransectionLog { get; set; }
    public virtual DbSet<UserLog> UserLog { get; set; }
    public virtual DbSet<RoleListLog> RoleListLog { get; set; }
    public virtual DbSet<MembershipLog> MembershipLog { get; set; }
    public virtual DbSet<UserMembershipMappingLog> UserMembershipMappingLog { get; set; }
    public virtual DbSet<StatusTypeLog> StatusTypeLog { get; set; }
    public virtual DbSet<BookFileMappingLog> BookFileMappingLog { get; set; }
    public virtual DbSet<PenaltyTypeLog> PenaltyTypeLog { get; set; }
    public virtual DbSet<OutboxMessagesLog> OutboxMessagesLog { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LibraryManagementSysContext).Assembly);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
