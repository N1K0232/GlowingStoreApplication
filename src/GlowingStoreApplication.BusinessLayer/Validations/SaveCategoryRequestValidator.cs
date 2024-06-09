using FluentValidation;
using GlowingStoreApplication.Shared.Models.Requests;

namespace GlowingStoreApplication.BusinessLayer.Validations;

public class SaveCategoryRequestValidator : AbstractValidator<SaveCategoryRequest>
{
    public SaveCategoryRequestValidator()
    {
        RuleFor(c => c.Name)
            .MaximumLength(256)
            .NotEmpty()
            .WithMessage("the name is required");

        RuleFor(c => c.Description).MaximumLength(512);
    }
}