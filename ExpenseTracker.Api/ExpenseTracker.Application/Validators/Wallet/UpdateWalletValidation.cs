using ExpenseTracker.Application.Requests.Wallet;
using FluentValidation;

namespace ExpenseTracker.Application.Validators.Wallet;

public sealed class UpdateWalletValidation : AbstractValidator<UpdateWalletRequest>
{
    public UpdateWalletValidation()
    {
        RuleFor(request => request.Id)
            .GreaterThan(0).WithMessage("Id must be greater than zero.");

        RuleFor(request => request.Name)
            .NotEmpty().WithMessage("Wallet name is required.")
            .MaximumLength(100).WithMessage("Wallet name cannot exceed 100 characters.");

        RuleFor(request => request.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        RuleFor(request => request.Balance)
            .GreaterThanOrEqualTo(0).WithMessage("Balance must be zero or a positive value.");

   

   
    }
}