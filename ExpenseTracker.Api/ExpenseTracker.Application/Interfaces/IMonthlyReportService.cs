namespace ExpenseTracker.Application.Interfaces;

public interface IMonthlyReportService
{
    Task SendMonthlyReportToAllUsersAsync();
}
