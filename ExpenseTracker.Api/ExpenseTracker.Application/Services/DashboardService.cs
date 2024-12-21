using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.Models;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Services;
internal sealed class DashboardService : IDashboardService
{
    private readonly IApplicationDbContext _context;

    public DashboardService(IApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<DashboardDto> GetDashboard()
    {
        var dto = new DashboardDto();

        PopulateWidgets(dto);
        PopulateSplineChartData(dto);
        PopulateDoughnutChart(dto);
        PopulateRecentTransactions(dto);

        return dto;
    }
    private void PopulateWidgets(DashboardDto dto)
    {
        var totalIncome = _context.Transfers
            .Where(x => x.Category.Type == CategoryType.Income)
            .Sum(x => x.Amount);
        var totalExpense = _context.Transfers
            .Where(x => x.Category.Type == CategoryType.Expense)
            .Sum(x => x.Amount);

        dto.Balance = totalIncome - totalExpense;
        dto.TotalIncome = totalIncome;
        dto.TotalExpense = totalExpense;
    }

    private void PopulateSplineChartData(DashboardDto dto)
    {
        var allTransfers = _context.Transfers
            .Where(x => x.Date < DateTime.UtcNow && x.Date > DateTime.UtcNow.AddDays(-7))
            .Include(x => x.Category)
            .AsNoTracking()
            .ToList();

        //Income
        List<SplineChart> incomeSummary = allTransfers
            .Where(x => x.Category.Type == CategoryType.Income)
            .GroupBy(j => j.Date)
            .Select(k => new SplineChart()
            {
                Day = k.First().Date.ToString("dd-MMM"),
                Income = k.Sum(l => l.Amount)
            })
            .ToList();

        //Expense
        List<SplineChart> expenseSummary = allTransfers
            .Where(x => x.Category.Type == CategoryType.Expense)
            .GroupBy(j => j.Date)
            .Select(k => new SplineChart()
            {
                Day = k.First().Date.ToString("dd-MMM"),
                Expense = k.Sum(l => l.Amount)
            })
            .ToList();

        //Combine Income & Expense
        //Last 7 Days
        DateTime startDate = DateTime.UtcNow.AddDays(-6);
        DateTime endDate = DateTime.UtcNow;

        var last7Days = Enumerable.Range(0, 7)
            .Select(i => startDate.AddDays(i).ToString("dd-MMM"))
            .ToList();

        var data = from day in last7Days
                   join expense in expenseSummary on day equals expense.Day into dayExpenseJoined
                   from totalExpense in dayExpenseJoined.DefaultIfEmpty()
                   join income in incomeSummary on day equals income.Day into dayIncomeJoined
                   from totalIncome in dayIncomeJoined.DefaultIfEmpty()
                   select new
                   {
                       Day = day,
                       Income = totalIncome,
                       Expense = totalExpense,
                   };

        dto.SpilneChartDatas = data
                        .Where(x => x.Income != null && x.Expense != null)
                        .GroupBy(x => x.Day)
                        .Select(x => new SplineChart(
                            x.Key,
                            x.Sum(x => x.Income.Income),
                            x.Sum(x => x.Expense.Expense))
                        )
                        .ToList();
    }

    private void PopulateDoughnutChart(DashboardDto dto)
    {
        dto.DoughnutChartDatas = _context.Transfers
     .Where(x => x.Category.Type == CategoryType.Expense) // Ushbu qism SQLda ishlaydi
     .GroupBy(j => j.Category.Id)
     .AsEnumerable() // Qolgan qismi xotirada ishlaydi
     .Select(k => new DoughnutChart
     (
         k.Sum(j => j.Amount),
         k.First().Category.Name,
         k.Sum(j => j.Amount).ToString("C0")
     ))
     .OrderByDescending(l => l.Amount)
     .ToList();

    }

    private void PopulateRecentTransactions(DashboardDto dto)
    {
        dto.RecentTransfers = _context.Transfers
            .Include(x => x.Category)
            .AsNoTracking()
            .OrderByDescending(x => x.Date)
            .Take(10)
            .Select(x => new TransferDto
            (
                x.Id,
                x.Title,
                x.Notes,
                x.Amount,
                x.Date,
                x.Category.Id,
                x.Category.Name,
                x.Wallet.Id,
                x.Wallet.Name
            )
            )
            .ToList();
    }
}
