using ExpenseTracker.Application.Requests.Auth;

namespace ExpenseTracker.Application.Interfaces;

public interface IAuthService
{
    Task<string> LoginAsync(LoginRequest request);
    Task RegisterAsync(RegisterRequest request);
    Task ConfirmEmailAsync(EmailConfirmationRequest request);
    Task ResetPasswordAsync(ResetPasswordRequest request);
    Task ConfirmResetPasswordAsync(ConfirmResetPasswordRequest request);
}