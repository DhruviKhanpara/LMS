using LMS.Core.Common;

namespace LMS.Core.Entities.LogEntities;

public class UserLog : LogAudit
{
    #region Table References
    public virtual RoleList? Role { get; set; }
    #endregion

    public long? Id { get; set; }
    public long? RoleId { get; set; }
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public DateTimeOffset? Dob { get; set; }
    public string? Gender { get; set; }
    public string? Address { get; set; }
    public string? MobileNo { get; set; }
    public string? Email { get; set; }
    public string? Username { get; set; }
    public byte[]? PasswordHash { get; set; }
    public byte[]? PasswordSolt { get; set; }
    public string? ProfilePhoto { get; set; }
    public string? LibraryCardNumber { get; set; }
    public DateTimeOffset? JoiningDate { get; set; }
}
