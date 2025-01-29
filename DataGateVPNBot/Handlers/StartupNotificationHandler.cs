using System.Reflection;
using DataGateVPNBot.Contexts;
using DataGateVPNBot.Services.Interfaces;
using Telegram.Bot;

namespace DataGateVPNBot.Handlers;


public class StartupNotificationHandler : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<StartupNotificationHandler> _logger;

    public StartupNotificationHandler(
        IServiceProvider serviceProvider,
        ILogger<StartupNotificationHandler> logger)
    {
        _serviceProvider = serviceProvider;
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
