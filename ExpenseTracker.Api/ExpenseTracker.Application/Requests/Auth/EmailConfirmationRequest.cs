namespace ExpenseTracker.Application.Requests.Auth;

public sealed record EmailConfirmationRequest(string Email, string Token);
