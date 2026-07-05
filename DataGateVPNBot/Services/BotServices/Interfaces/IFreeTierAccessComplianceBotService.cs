using DataGateVPNBot.Services.BotServices;

namespace DataGateVPNBot.Services.BotServices.Interfaces;

public interface IFreeTierAccessComplianceBotService
{
    Task<bool?> IsSubscribedToRequiredChannelAsync(long telegramId, CancellationToken cancellationToken);

    /// <summary>
    /// For Free/Default plans: requires merged Telegram+Google account or channel subscription.
    /// Notifies admins when access is denied. Returns a user-facing message when blocked.
    /// </summary>
    Task<VpnAccessGateResult> EnsureVpnAccessAsync(long telegramId, string context, CancellationToken cancellationToken);
}
