using ExpenseTracker.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Middlewares;

internal sealed class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    public ErrorHandlerMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleAsync(ex, context);
        }
    }

    private async Task HandleAsync(Exception exception, HttpContext context)
    {
        _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

        var details = GetErrorDetails(exception);

        context.Response.StatusCode = details.Status!.Value;

        await context.Response
            .WriteAsJsonAsync(details);
    }

    private static ProblemDetails GetErrorDetails(Exception exception)
        => exception switch
        {
            EntityNotFoundException => new ProblemDetails { Status = StatusCodes.Status404NotFound, Title = "Not Found", Detail = exception.Message },
            UserNameAlreadyTakenException => new ProblemDetails { Status = StatusCodes.Status400BadRequest, Title = "User Name already taken", Detail = exception.Message },
            InvalidLoginRequestException => new ProblemDetails { Status = StatusCodes.Status401Unauthorized, Title = "Invalid username or password", Detail = "Verify that the account with given username exists and password matches" },
            _ => new ProblemDetails { Status = StatusCodes.Status500InternalServerError, Title = "Internal Server Error", Detail = exception.Message }
        };
}
