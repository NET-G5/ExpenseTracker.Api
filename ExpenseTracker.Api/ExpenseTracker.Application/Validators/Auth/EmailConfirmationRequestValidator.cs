using ExpenseTracker.Application.Requests.Auth;
using FluentValidation;

namespace ExpenseTracker.Application.Validators.Auth;
public class EmailConfirmationRequestValidator : AbstractValidator<EmailConfirmationRequest>
{
    public EmailConfirmationRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email cannot be empty")
            .EmailAddress()
            .WithMessage("Invalid email address");

        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Token cannot be empty");
    }
}
