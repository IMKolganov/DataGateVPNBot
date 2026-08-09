using System.Security.Authentication;
using DataGateVPNBot.Services.BotServices.Interfaces;
using DataGateVPNBot.Services.DashboardServices.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DataGateVPNBot.Handlers;

public partial class TelegramUpdateHandler
{
    private async Task<Message> AdminUnsubscribedVpnUsersDigestAsync(
        Message msg,
        CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var tgUserService = scope.ServiceProvider.GetRequiredService<ITelegramBotUserService>();

        if (!await tgUserService.IsTelegramDashboardAdminAsync(msg.From!.Id, cancellationToken))
        {
            return await _botClient.SendMessage(
                msg.Chat.Id,
                "⛔ This command is only for bot administrators.",
                cancellationToken: cancellationToken);
        }

        await _botClient.SendMessage(
            msg.Chat.Id,
            "⏳ Building Free/Default unsubscribed VPN digest…",
            cancellationToken: cancellationToken);

        try
        {
            var digestService = scope.ServiceProvider.GetRequiredService<IFreeTierUnsubscribedVpnDigestBotService>();
            var text = await digestService.GetDigestTextAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(text))
            {
                return await _botClient.SendMessage(
                    msg.Chat.Id,
                    "Could not load the digest from the dashboard API.",
                    cancellationToken: cancellationToken);
            }

            if (text.Length > 4090)
                text = text[..4090] + "…";

            return await _botClient.SendMessage(
                msg.Chat.Id,
                text,
                cancellationToken: cancellationToken);
        }
        catch (AuthenticationException ex)
        {
            _logger.LogWarning(ex, "Admin unsubscribed VPN digest: authentication failed");
            return await _botClient.SendMessage(
                msg.Chat.Id,
                "❌ Dashboard authentication failed. Try again later.",
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Admin unsubscribed VPN digest failed");
            return await _botClient.SendMessage(
                msg.Chat.Id,
                "❌ Failed to load the digest. Check backend logs.",
                cancellationToken: cancellationToken);
        }
    }
}
