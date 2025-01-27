using DataGateVPNBotV1.Services.Interfaces;

namespace DataGateVPNBotV1.Services;

public class OpenVpnBackgroundService : BackgroundService, IOpenVpnBackgroundService
{
    private readonly ILogger<OpenVpnBackgroundService> _logger;
    private readonly IOpenVpnParserService _parserService;
    private const int Seconds = 60;

    public OpenVpnBackgroundService(ILogger<OpenVpnBackgroundService> logger, IOpenVpnParserService parserService)
    {
        _logger = logger;
        _parserService = parserService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OpenVPN Background Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Parsing OpenVPN status file at {Time}", DateTimeOffset.Now);
                await _parserService.ParseAndSaveAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,$"Error occurred while parsing OpenVPN status file. Message: {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromSeconds(Seconds), stoppingToken);
        }

        _logger.LogInformation("OpenVPN Background Service is stopping.");
    }
}