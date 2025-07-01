using System.Net;
using System.Text.Json;

namespace Breach.Api.Middleware;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse();

        switch (exception)
        {
            case ArgumentNullException:
            case ArgumentException:
                response.Message = "Invalid request parameters";
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;
            
            case UnauthorizedAccessException:
                response.Message = "Unauthorized access";
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                break;
            
            case TimeoutException:
                response.Message = "Request timeout";
                response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                context.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                break;
            
            default:
                response.Message = "An error occurred while processing your request";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(jsonResponse);
    }
}

public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}