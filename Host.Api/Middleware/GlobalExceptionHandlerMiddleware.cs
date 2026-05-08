using FluentValidation;
using System.Net;
using System.Text.Json;

namespace Host.Api.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
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
        catch (ValidationException ex)
        {
            _logger.LogWarning("Validation failed: {Errors}", ex.Errors);
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            var errors = ex.Errors.Select(e => new
            {
                Field = e.PropertyName,
                Error = e.ErrorMessage
            });

            var response = new
            {
                Type = "ValidationError",
                Title = "One or more validation errors occurred.",
                Status = 400,
                Errors = errors
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred.");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new
            {
                Type = "ServerError",
                Title = "An unexpected error occurred.",
                Status = 500
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
