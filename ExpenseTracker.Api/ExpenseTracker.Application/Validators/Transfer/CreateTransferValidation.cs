using ExpenseTracker.Application.Requests.Transfer;
using FluentValidation;

namespace ExpenseTracker.Application.Validators.Transfer;

public sealed class CreateTransferValidation : AbstractValidator<CreateTranferRequest>
{
    public CreateTransferValidation()
    {
        RuleFor(request => request.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

        RuleFor(request => request.Notes)
            .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters.");

        RuleFor(request => request.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");

        RuleFor(request => request.Date)
            .NotEmpty().WithMessage("Date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Date cannot be in the future.");

        RuleFor(request => request.CategoryId)
            .GreaterThan(0).WithMessage("CategoryId must be greater than zero.");

        RuleFor(request => request.WalletId)
            .GreaterThan(0).WithMessage("WalletId must be greater than zero.");
    }
}