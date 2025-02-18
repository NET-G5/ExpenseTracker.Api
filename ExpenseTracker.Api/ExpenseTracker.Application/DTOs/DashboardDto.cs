﻿using ExpenseTracker.Application.Models;

namespace ExpenseTracker.Application.DTOs;

public sealed class DashboardDto
{
    public decimal Balance { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public List<SplineChart> SpilneChartData { get; set; }
    public List<DoughnutChart> DoughnutChartData { get; set; }
    public List<TransferDto> RecentTransfers { get; set; }
}
