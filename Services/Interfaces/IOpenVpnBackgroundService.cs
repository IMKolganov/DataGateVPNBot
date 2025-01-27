namespace DataGateVPNBotV1.Services;

public interface IOpenVpnBackgroundService
{
    Task StartAsync(CancellationToken cancellationToken);
    Task StopAsync(CancellationToken cancellationToken);
    void Dispose();
    Task? ExecuteTask { get; }
}