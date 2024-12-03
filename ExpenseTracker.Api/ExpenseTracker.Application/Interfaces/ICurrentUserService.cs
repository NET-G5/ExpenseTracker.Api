namespace ExpenseTracker.Application.Interfaces;

public interface ICurrentUserService
{
    Guid GetCurrentUserId();
}