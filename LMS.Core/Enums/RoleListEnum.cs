
using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
 
namespace LMS.Core.Enums;

/// <summary>
/// RoleList auto generated enumeration
/// </summary>
[GeneratedCode("TextTemplatingFileGenerator", "10")]
public enum RoleListEnum : long
{
    [Display(Name = "Admin", Description = "Can handle Librarian and all rights of them")]
    Admin = 1,

    [Display(Name = "Librarian", Description = "Can handle User and manage books and there transaction")]
    Librarian = 2,

    [Display(Name = "User", Description = "User of the library can borrow book have not right to manage anything")]
    User = 3
}

