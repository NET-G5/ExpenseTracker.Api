using ExpenseTracker.Application.Requests.Auth;
using ExpenseTracker.Application.Responses.Auth;

namespace ExpenseTracker.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<RefreshTokenResponse> RefreshAsync(RefreshTokenRequest request);
    Task RegisterAsync(RegisterRequest request);
    Task ConfirmEmailAsync(EmailConfirmationRequest request);
    Task ResetPasswordAsync(ResetPasswordRequest request);
    Task ConfirmResetPasswordAsync(ConfirmResetPasswordRequest request);
}