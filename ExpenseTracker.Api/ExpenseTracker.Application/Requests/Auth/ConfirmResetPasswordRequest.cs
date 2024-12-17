namespace ExpenseTracker.Application.Requests.Auth;

public sealed record ConfirmResetPasswordRequest(
    string Email,
    string Token,
    string NewPassword,
    string ConfirmPassword);
