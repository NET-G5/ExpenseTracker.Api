using ExpenseTracker.Application.DTOs;

namespace ExpenseTracker.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardDto> GetDashboard();
}