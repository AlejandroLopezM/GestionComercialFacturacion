using GestionComercialFacturacion.Api.Exceptions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GestionComercialFacturacion.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
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
        catch (ApiException ex)
        {
            _logger.LogWarning(
                ex,
                "Error controlado. Error: {Error}. Message: {Message}",
                ex.Error,
                ex.Message);

            await WriteErrorAsync(
                context,
                ex.StatusCode,
                ex.Error,
                ex.Message,
                ex.Details);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado en la API.");

            await WriteErrorAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "UnexpectedError",
                "Ha ocurrido un error inesperado.");
        }
    }

    private static async Task WriteErrorAsync(
        HttpContext context,
        int statusCode,
        string error,
        string message,
        IReadOnlyList<string>? details = null)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = new ErrorResponse
        {
            Error = error,
            Message = message,
            Details = details is { Count: > 0 } ? details : null
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });

        await context.Response.WriteAsync(json);
    }

    private sealed class ErrorResponse
    {
        public string Error { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public IReadOnlyList<string>? Details { get; set; }
    }
}