namespace DataGateVPNBot.Services.BotServices.Interfaces;

public interface IFreeTierUnsubscribedVpnDigestBotService
{
    /// <summary>
    /// Fetches the on-demand unsubscribed VPN digest text from the dashboard API.
    /// </summary>
    Task<string?> GetDigestTextAsync(CancellationToken cancellationToken);
}
