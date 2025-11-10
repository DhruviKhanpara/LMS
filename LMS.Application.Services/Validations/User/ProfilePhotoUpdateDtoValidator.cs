using FluentValidation;
using LMS.Application.Contracts.DTOs.User;

namespace LMS.Application.Services.Validations.User;

public class ProfilePhotoUpdateDtoValidator : AbstractValidator<ProfilePhotoUpdateDto>
{
    public ProfilePhotoUpdateDtoValidator()
    {
        RuleFor(user => user.ProfilePhoto)
            .Must(file => file is null || file.ContentType == "image/jpeg" || file.ContentType == "image/jpg" || file.ContentType == "image/png")
            .WithMessage("Profile photo file must be a JPEG or PNG image.");

        RuleFor(user => user.IsDeletedProfile)
            .Must(value => value == true || value == false).WithMessage("IsDeletedProfile flag is required");
    }
}
