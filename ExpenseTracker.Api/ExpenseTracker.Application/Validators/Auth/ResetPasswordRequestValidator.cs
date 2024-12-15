using ExpenseTracker.Application.Requests.Auth;
using FluentValidation;

namespace ExpenseTracker.Application.Validators.Auth;
public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email cannot be empty")
            .EmailAddress()
            .WithMessage("Invalid email address");

        RuleFor(x => x.RedirectUrl)
           .NotEmpty()
           .WithMessage("Redirect Url cannot be empty");

        RuleFor(x => x.OS)
            .NotEmpty()
            .WithMessage("Operating system cannot be empty")
            .Unless(x => x.OS == null);

        RuleFor(x => x.Browser)
            .NotEmpty()
            .WithMessage("Browser cannot be empty")
            .Unless(x => x.Browser == null);
    }
}
