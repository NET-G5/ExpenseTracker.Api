namespace ExpenseTracker.Application.Requests.Auth;

public sealed record RegisterRequest(
    string Email,
    string UserName,
    string Password,
    string ConfirmPassword,
    string ConfirmUrl,
    string? PhoneNumber,
    string? OS,
    string? Browser);