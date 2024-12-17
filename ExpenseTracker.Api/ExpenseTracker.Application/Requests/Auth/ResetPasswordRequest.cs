namespace ExpenseTracker.Application.Requests.Auth;

public sealed record ResetPasswordRequest(
    string Email,
    string RedirectUrl,
    string? OS,
    string? Browser);
