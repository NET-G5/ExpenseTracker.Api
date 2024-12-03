using ExpenseTracker.Application.Requests.Wallet;
using FluentValidation;

namespace ExpenseTracker.Application.Validators.Wallet;

public sealed class CreateWalletValidation : AbstractValidator<CreateWalletRequest>
{
    public CreateWalletValidation()
    {
        RuleFor(request => request.Name)
            .NotEmpty().WithMessage("Wallet name is required.")
            .MaximumLength(100).WithMessage("Wallet name cannot exceed 100 characters.");

        RuleFor(request => request.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        RuleFor(request => request.Balance)
            .GreaterThanOrEqualTo(0).WithMessage("Balance must be zero or a positive value.");

        RuleFor(request => request.OwnerId)
            .NotEmpty().WithMessage("OwnerId is required.")
            .Must(ownerId => ownerId != Guid.Empty).WithMessage("OwnerId must be a valid GUID.");

        RuleFor(request => request.Owner)
            .NotNull().WithMessage("Owner is required.");

        RuleFor(request => request.Transfers)
            .NotNull().WithMessage("Transfers cannot be null.")
            .Must(transfers => transfers.All(t => t != null)).WithMessage("Transfers must contain valid items.");
    }
}