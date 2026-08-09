using DataGateMonitor.SharedModels.DataGateMonitor.FreeTierEnforcement.Responses;

namespace DataGateVPNBot.Services.BotServices.Interfaces;

public interface IFreeTierUnsubscribedVpnDigestBotService
{
    Task<FreeTierUnsubscribedVpnDigestResponse?> GetDigestAsync(CancellationToken cancellationToken);
}
