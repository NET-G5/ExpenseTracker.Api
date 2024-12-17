using ExpenseTracker.Api.Extensions;
using ExpenseTracker.Application.Constants;
using ExpenseTracker.Application.Extensions;
using ExpenseTracker.Application.Interfaces;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .RegisterApplication(builder.Configuration)
    .RegisterApi(builder.Configuration);

builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDatabaseSeeder();
}

app.UseHangfireDashboard();

RecurringJob.AddOrUpdate<IMonthlyReportService>(
    BackgroundJobConstants.MonthlyReportId,
    job => job.SendMonthlyReportToAllUsersAsync(),
    Cron.Monthly(1));

app.UseErrorHandler();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();