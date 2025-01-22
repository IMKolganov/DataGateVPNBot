using System.Reflection;
using DataGateVPNBotV1.Contexts;
using DataGateVPNBotV1.Services.Interfaces;
using Telegram.Bot;

namespace DataGateVPNBotV1.Handlers;


public class StartupNotificationHandler : IHostedService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostEnvironment _environment;
    private readonly ILogger<StartupNotificationHandler> _logger;

    public StartupNotificationHandler(
        ITelegramBotClient botClient,
        IServiceProvider serviceProvider,
        IHostEnvironment environment,
        ILogger<StartupNotificationHandler> logger)
    {
        _botClient = botClient;
        _serviceProvider = serviceProvider;
        _environment = environment;
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
            var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown version";

            var startupMessage = $"🚀 Bot started successfully!\n" +
                                 $"Application version: {version}\n" +
                                 $"Environment: {_environment.EnvironmentName}\n" +
                                 $"Time: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";

            await _botClient.SendMessage(admin.TelegramId, startupMessage, cancellationToken: cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
