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
        var errorService = scope.ServiceProvider.GetRequiredService<IErrorService>();
        await errorService.NotifyAdminsAboutStartAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
