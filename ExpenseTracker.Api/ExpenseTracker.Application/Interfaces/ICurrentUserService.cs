namespace ExpenseTracker.Application.Interfaces;

public interface ICurrentUserService
{
    Guid GetUserId();
    string GetUserName();
}