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
        .WriteTo.File("logs/startup_logs.txt", LogEventLevel.Information)
        .CreateBootstrapLogger();

    var builder = WebApplication.CreateBuilder(args);

    Log.Logger.Information("API is starting...");

    builder.Logging.ClearProviders();
    builder.Host.UseSerilog((context, _, configuration) => 
        configuration.ReadFrom.Configuration(context.Configuration));

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

    Log.Logger.Information("Application is running...");

    app.Run();
}
catch (Exception ex)
{
    Log.Logger.Error(ex, "Unexpected error occurred during application startup. {Message}", ex.Message);
    throw;
}
