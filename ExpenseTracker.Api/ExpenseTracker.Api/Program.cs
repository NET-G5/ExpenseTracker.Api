using ExpenseTracker.Api.Extensions;
using ExpenseTracker.Application.Constants;
using ExpenseTracker.Application.Extensions;
using ExpenseTracker.Application.Interfaces;
using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Serilog;
using Serilog.Events;

try
{
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.Console()
        .WriteTo.File("logs/logs_.txt", LogEventLevel.Information, rollingInterval: RollingInterval.Day)
        .WriteTo.File("logs/errors_.txt", LogEventLevel.Error, rollingInterval: RollingInterval.Day)
        .CreateLogger();

    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Host.UseSerilog();

    builder.Services
        .RegisterApplication(builder.Configuration)
        .RegisterApi(builder.Configuration);

    builder.Services.AddHttpClient();

    var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDatabaseSeeder();

    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = [ new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
        {
            RequireSsl = false,
            SslRedirect = false,
            LoginCaseSensitive = true,
            Users =
            [
                new BasicAuthAuthorizationUser
                {
                    Login = "admin",
                    PasswordClear =  "admin"
                }
            ]

        }) ]
    });

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
}
catch (Exception ex)
{
    Log.Error(ex.Message);
}
