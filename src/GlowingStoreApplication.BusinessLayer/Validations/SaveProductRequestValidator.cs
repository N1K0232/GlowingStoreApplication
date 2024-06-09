using FluentValidation;
using GlowingStoreApplication.Shared.Models.Requests;

namespace GlowingStoreApplication.BusinessLayer.Validations;

public class SaveProductRequestValidator : AbstractValidator<SaveProductRequest>
{
    public SaveProductRequestValidator()
    {
        RuleFor(p => p.CategoryId)
            .NotEmpty()
            .WithMessage("the category is required");

        RuleFor(p => p.Name)
            .MaximumLength(256)
            .NotEmpty()
            .WithMessage("the name is required");

        RuleFor(p => p.Description)
            .NotEmpty()
            .WithMessage("the description is required");

        RuleFor(p => p.Quantity)
            .NotEmpty()
            .WithMessage("the quantity is required");

        RuleFor(p => p.Price)
            .PrecisionScale(8, 2, true)
            .WithMessage("insert a valid price")
            .NotEmpty()
            .WithMessage("the price is required");

        RuleFor(p => p.DiscountPercentage)
            .GreaterThan(0)
            .When(p => p.DiscountPercentage.HasValue)
            .WithMessage("the discount percentage must be greater than 0");
    }
}