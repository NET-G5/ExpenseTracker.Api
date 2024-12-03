using ExpenseTracker.Application.Requests.Category;
using FluentValidation;

namespace ExpenseTracker.Application.Validators.Category;

public sealed class CreateCategoryValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name must be specified.")
            .Length(5, 255).WithMessage("Category name must be between 5 and 255 characters.");

        RuleFor(x => x.Description)
            .Length(5, 500).WithMessage("Description must be between 5 and 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid category type provided.");
    }
}