using LMS.Core.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Core.Entities;

public partial class User : Audit
{
    #region Table References
    [ForeignKey(nameof(RoleId))]
    public virtual RoleList Role { get; set; } = null!;
    public virtual List<Penalty> Penalties { get; set; } = new List<Penalty>();
    public virtual List<Transection> Transections { get; set; } = new List<Transection>();
    public virtual List<Reservation> Reservations { get; set; } = new List<Reservation>();
    public virtual List<UserMembershipMapping> UserMemberships { get; set; } = new List<UserMembershipMapping>();
    #endregion

    public long Id { get; set; }
    public long RoleId { get; set; }
    public string FirstName { get; set; } = null!;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = null!;
    public DateTimeOffset Dob { get; set; }
    public string Gender { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string MobileNo { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Username { get; set; } = null!;
    public byte[] PasswordHash { get; set; } = null!;
    public byte[] PasswordSolt { get; set; } = null!;
    public string? ProfilePhoto { get; set; }
    public string? LibraryCardNumber { get; set; }
    public DateTimeOffset JoiningDate { get; set; }
    [ForeignKey(nameof(CreatedBy))]
    public new long? CreatedBy { get; set; }
}
