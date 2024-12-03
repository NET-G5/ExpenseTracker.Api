using ExpenseTracker.Api.Helpers;
using ExpenseTracker.Api.Middlewares;
using ExpenseTracker.Domain.Interfaces;

namespace ExpenseTracker.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseDatabaseSeeder(this WebApplication app)
    {
        var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        DatabaseSeeder.SeedDatabase(context);

        return app;
    }

    public static WebApplication UseErrorHandler(this WebApplication app)
    {
        app.UseMiddleware<ErrorHandlerMiddleware>();

        return app;
    }
}
