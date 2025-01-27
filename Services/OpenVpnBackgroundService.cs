using DataGateVPNBotV1.Services.Interfaces;

namespace DataGateVPNBotV1.Services;

public class OpenVpnBackgroundService : BackgroundService, IOpenVpnBackgroundService
{
    private readonly ILogger<OpenVpnBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private const int Seconds = 60;

    public OpenVpnBackgroundService(ILogger<OpenVpnBackgroundService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OpenVPN Background Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var parserService = scope.ServiceProvider.GetRequiredService<IOpenVpnParserService>();
                try
                {
                    _logger.LogInformation("Parsing OpenVPN status file at {Time}", DateTimeOffset.Now);
                    await parserService.ParseAndSaveAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while parsing OpenVPN status file.");
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }

        _logger.LogInformation("OpenVPN Background Service is stopping.");
    }
}