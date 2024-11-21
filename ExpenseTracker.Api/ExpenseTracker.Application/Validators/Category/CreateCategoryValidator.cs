using ExpenseTracker.Application.Requests.Category;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Application.Validators.Category;

public sealed class CreateCategoryValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryValidator()
    {
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
