namespace DataGateVPNBotV1.Services.BotServices.Interfaces;

public interface IOpenVpnBackgroundService
{
    Task StartAsync(CancellationToken cancellationToken);
    Task StopAsync(CancellationToken cancellationToken);
    void Dispose();
    Task? ExecuteTask { get; }
}