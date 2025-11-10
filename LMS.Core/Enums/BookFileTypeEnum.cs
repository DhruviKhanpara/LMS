using System.ComponentModel.DataAnnotations;

namespace LMS.Core.Enums;

public enum BookFileTypeEnum : long
{
    [Display(Name = "CoverPage", Description = "Book's Cover Page file")]
    CoverPage = 1,

    [Display(Name = "BookPreview", Description = "Book Preview file")]
    BookPreview = 2,
}
