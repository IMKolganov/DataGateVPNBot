using System.Net;
using DataGateVPNBotV1.Contexts;
using DataGateVPNBotV1.Models;
using DataGateVPNBotV1.Services.Interfaces;
using Newtonsoft.Json;
using Telegram.Bot;

namespace DataGateVPNBotV1.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IErrorService _errorService;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, IErrorService errorService, 
        ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _errorService = errorService;
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
            _logger.LogError(ex, "Unhandled exception occurred.");
            await _errorService.LogErrorToDatabase(ex, context);
            await _errorService.NotifyAdminsAsync(ex, context);
            await HandleExceptionAsync(context, ex);

        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = new
        {
            StatusCode = context.Response.StatusCode,
            Message = "An unexpected error occurred. Please try again later.",
            // Detail = exception.Message
        };

        return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
    }
}