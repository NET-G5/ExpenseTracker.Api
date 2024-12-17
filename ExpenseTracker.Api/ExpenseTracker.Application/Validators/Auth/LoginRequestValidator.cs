using ExpenseTracker.Application.Requests.Auth;
using FluentValidation;

namespace ExpenseTracker.Application.Validators.Auth;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("User name cannot be empty.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password cannot be empty.");
    }
}
