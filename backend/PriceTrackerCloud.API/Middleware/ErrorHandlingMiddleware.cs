using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace PriceTrackerCloud.API.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next   = next;
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
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await WriteProblemDetailsAsync(context, ex);
        }
    }

    private static async Task WriteProblemDetailsAsync(HttpContext context, Exception ex)
    {
        var (status, title, detail) = ex switch
        {
            ValidationException ve      => (400, "Validation Error",
                string.Join("; ", ve.Errors.Select(e => e.ErrorMessage))),
            UnauthorizedAccessException => (401, "Unauthorized",           ex.Message),
            InvalidOperationException   => (409, "Conflict",               ex.Message),
            KeyNotFoundException        => (404, "Not Found",              ex.Message),
            _                          => (500, "Internal Server Error",   "An unexpected error occurred.")
        };

        var problem = new ProblemDetails
        {
            Type   = "https://tools.ietf.org/html/rfc7807",
            Title  = title,
            Status = status,
            Detail = detail
        };

        context.Response.StatusCode  = status;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(problem, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
    }
}
