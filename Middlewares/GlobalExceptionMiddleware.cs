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
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            _logger.LogError(ex, "Unhandled exception occurred.");
            await LogErrorToDatabase(ex, context);
            await NotifyAdminsAsync(ex, context);
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
    
    private async Task LogErrorToDatabase(Exception exception, HttpContext context)
    {
        try
        {
            using var scope = context.RequestServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var errorLog = new ErrorLog
            {
                Message = exception.Message,
                StackTrace = exception.StackTrace ?? string.Empty,
                Timestamp = DateTime.UtcNow,
                Source = context.Request.Path
            };

            dbContext.ErrorLogs.Add(errorLog);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log error to the database.");
        }
    }
    
    private async Task NotifyAdminsAsync(Exception exception, HttpContext context)
    {
        using var scope = context.RequestServices.CreateScope();
        var telegramUsersService = scope.ServiceProvider.GetRequiredService<ITelegramUsersService>();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

        var admins = await telegramUsersService.GetAdminsAsync();

        if (admins is { Count: 0 })
        {
            _logger.LogWarning("No admins are configured to receive error notifications.");
            return;
        }

        _logger.LogInformation($"Notifying {admins!.Count} admins about an error.");

        foreach (var admin in admins)
        {
            try
            {
                var errorMessage = $"🚨 *Error Notification*\n" +
                                   $"Path: `{context.Request.Path}`\n" +
                                   $"Message: `{exception.Message}`\n" +
                                   $"Time: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC\n" +
                                   $"Stack Trace:\n```{exception.StackTrace}```";

                await botClient.SendMessage(admin.TelegramId, errorMessage, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send error notification to admin with Telegram ID {admin.TelegramId}.");
            }
        }
    }

}