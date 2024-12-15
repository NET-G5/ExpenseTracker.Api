using ExpenseTracker.Application.Requests.Auth;
using FluentValidation;

namespace ExpenseTracker.Application.Validators.Auth;

public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email cannot be empty.")
            .EmailAddress()
            .WithMessage("Invalid email address.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password cannot be empty.")
            .Must((request, password) => request.ConfirmPassword.Equals(password))
            .WithMessage("Password and confirm password do not match.")
            .MinimumLength(8)
            .WithMessage("Password is too short.");

        RuleFor(x => x.ConfirmUrl)
            .NotEmpty()
            .WithMessage("Confirmation URL cannot be empty.");

        RuleFor(x => x.PhoneNumber)
            .Must((request, phoneNumber, context) =>
            {
                if (phoneNumber!.Length == 9)
                {
                    return phoneNumber.All(char.IsDigit);
                }

                if (phoneNumber.Length == 13 && phoneNumber.StartsWith("+998"))
                {
                    return phoneNumber[4..].All(char.IsDigit);
                }

                return false;
            })
            .When(x => x.PhoneNumber is not null)
            .WithMessage("Invalid phone number.");

        RuleFor(x => x.OS)
            .NotEmpty()
            .Unless(x => x.OS == null);
    }
}
