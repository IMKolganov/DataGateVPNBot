using DataGateMonitor.SharedModels.DataGateMonitor.FreeTierEnforcement.Enums;
using DataGateMonitor.SharedModels.DataGateMonitor.FreeTierEnforcement.Responses;

namespace DataGateVPNBot.Services.BotServices.Interfaces;

public interface IFreeTierChannelSubscribeRemindBotService
{
    Task<FreeTierChannelSubscribeRemindResponse> RemindAsync(
        string target,
        FreeTierChannelSubscribeRemindChannel channel,
        CancellationToken cancellationToken);
}
