namespace ExpenseTracker.Application.Requests.Auth;

public sealed record LoginRequest(string UserName, string Password);