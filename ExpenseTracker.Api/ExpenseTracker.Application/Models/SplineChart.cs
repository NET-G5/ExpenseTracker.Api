namespace ExpenseTracker.Application.Models;

public sealed class SplineChart
{
    public string Day { get; set; }
    public decimal Income { get; set; }
    public decimal Expense { get; set; }

    public SplineChart(string day, decimal income, decimal expense)
    {
        Day = day;
        Income = income;
        Expense = expense;
    }

    public SplineChart()
    {

    }
}
