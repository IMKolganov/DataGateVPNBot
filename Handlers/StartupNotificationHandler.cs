using DataGateVPNBotV1.Contexts;
using DataGateVPNBotV1.Services.Interfaces;
using Telegram.Bot;

namespace DataGateVPNBotV1.Handlers;


public class StartupNotificationHandler : IHostedService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<StartupNotificationHandler> _logger;

    public StartupNotificationHandler(
        ITelegramBotClient botClient,
        IServiceProvider serviceProvider,
        ILogger<StartupNotificationHandler> logger)
    {
        _botClient = botClient;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var telegramUsersService = scope.ServiceProvider.GetRequiredService<ITelegramUsersService>();
        var admins = await telegramUsersService.GetAdminsAsync();

        if (admins.Count == 0)
        {
            _logger.LogWarning("Admin chat ID is not configured.");
            return;
        }
        _logger.LogInformation("Admins count: {RecordCount}", admins.Count);
        foreach (var admin in admins)
        {
            

            var startupMessage = $"🚀 Bot started successfully!\n" +
                                 $"Time: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";

            await _botClient.SendMessage(admin.TelegramId, startupMessage, cancellationToken: cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
