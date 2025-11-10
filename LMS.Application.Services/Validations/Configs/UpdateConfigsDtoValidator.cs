using FluentValidation;
using LMS.Application.Contracts.DTOs.Configs;

namespace LMS.Application.Services.Validations.Configs;

public class UpdateConfigsDtoValidator : AbstractValidator<UpdateConfigsDto>
{
    public UpdateConfigsDtoValidator()
    {
        RuleFor(config => config.Id)
            .NotEmpty().WithMessage("Config Id is required")
            .GreaterThan(0).WithMessage("config Id ,must be grester than 0");

        RuleFor(config => config.KeyName)
            .NotEmpty().WithMessage("KeyName is required");

        RuleFor(config => config.KeyValue)
            .NotEmpty().WithMessage("Key value is required");

        RuleFor(config => config.Description)
            .NotEmpty().WithMessage("Description is required");
    }
}
