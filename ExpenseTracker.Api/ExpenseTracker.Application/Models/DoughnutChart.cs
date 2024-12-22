namespace ExpenseTracker.Application.Models;

public sealed class DoughnutChart
{
    public decimal Amount { get; set; }
    public string CategoryName { get; set; }
    public string FormattedAmount { get; set; }

    public DoughnutChart(decimal amount, string categoryName, string formattedAmount)
    {
        Amount = amount;
        CategoryName = categoryName;
        FormattedAmount = formattedAmount;
    }

    public DoughnutChart()
    {

    }
}
