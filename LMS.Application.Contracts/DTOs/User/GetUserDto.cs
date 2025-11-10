using System.ComponentModel.DataAnnotations;

namespace LMS.Application.Contracts.DTOs.User;

public class GetUserDto
{
    public long Id { get; set; }
    public string? ProfilePhoto { get; set; }
    public string FirstName { get; set; } = null!;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = null!;
    [Display(Name = "Date of Birth")]
    public DateTimeOffset Dob { get; set; }
    public string Gender { get; set; } = null!;
    public string Address { get; set; } = null!;
    [Display(Name = "Mobile Number")]
    public string MobileNo { get; set; } = null!;
    [Display(Name = "E-mail")]
    public string Email { get; set; } = null!;
    public string Username { get; set; } = null!;
    public long RoleId { get; set; }
    public string Role { get; set; } = null!;
    [Display(Name = "Library card Number")]
    public string? LibraryCardNumber { get; set; }
    [Display(Name = "Joining Date")]
    public DateTimeOffset JoiningDate { get; set; }
    [Display(Name = "Has Penalty?")]
    public bool HasPenalty { get; set; }
    [Display(Name = "Occupied Book?")]
    public bool HasOccupiedBook { get; set; }
    public bool IsRemoved { get; set; } = false;
}
