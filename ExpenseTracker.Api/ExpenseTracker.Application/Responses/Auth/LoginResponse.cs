namespace ExpenseTracker.Application.Responses.Auth;

public sealed record LoginResponse(string AccessToken, string RefreshToken);
