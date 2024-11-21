using ExpenseTracker.Application.Requests.Category;
using FluentValidation;

namespace ExpenseTracker.Application.Validators.Category;

public sealed class UpdateCategoryValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage(x => $"Invalid id: {x.Id}.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Category name must be specified.")
            .MinimumLength(5)
            .WithMessage("Category name must have at least 5 characters.")
            .MaximumLength(255)
            .WithMessage("Category name must have at most 255 characters.");

        RuleFor(x => x.Description)
            .MinimumLength(5)
            .WithMessage("Description must have at least 5 characters.")
            .When(x => x.Description != null)
            .MaximumLength(500)
            .WithMessage("Description must have at most 500 characters.")
            .When(x => x.Description != null);
    }
}
