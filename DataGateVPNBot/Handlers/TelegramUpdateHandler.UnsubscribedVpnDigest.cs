using System.Security.Authentication;
using DataGateVPNBot.Services.BotServices.Interfaces;
using DataGateVPNBot.Services.DashboardServices.Interfaces;
using DataGateMonitor.SharedModels.DataGateMonitor.FreeTierEnforcement.Dto;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

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
            var digest = await digestService.GetDigestAsync(cancellationToken);

            if (digest is null || string.IsNullOrWhiteSpace(digest.Text))
            {
                return await _botClient.SendMessage(
                    msg.Chat.Id,
                    "Could not load the digest from the dashboard API.",
                    cancellationToken: cancellationToken);
            }

            var liveText = digest.Text.StartsWith("📋", StringComparison.Ordinal) ||
                           digest.Text.StartsWith("📅", StringComparison.Ordinal)
                ? "🔎 Live snapshot\n" + digest.Text
                : "🔎 Live snapshot\n\n" + digest.Text;

            if (liveText.Length > 4090)
                liveText = liveText[..4090] + "…";

            var keyboard = BuildEmailRemindKeyboard(digest.Candidates);
            return await _botClient.SendMessage(
                msg.Chat.Id,
                liveText,
                replyMarkup: keyboard,
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

    public static InlineKeyboardMarkup? BuildEmailRemindKeyboard(
        IReadOnlyList<FreeTierEnforcementCandidateDto>? candidates)
    {
        if (candidates is null || candidates.Count == 0)
            return null;

        var withEmail = candidates
            .Where(c => !string.IsNullOrWhiteSpace(c.Email))
            .OrderBy(c => c.DisplayName)
            .Take(24)
            .ToList();
        if (withEmail.Count == 0)
            return null;

        var rows = new List<InlineKeyboardButton[]>();
        const int perRow = 2;
        for (var i = 0; i < withEmail.Count; i += perRow)
        {
            var chunk = withEmail.Skip(i).Take(perRow)
                .Select(c => InlineKeyboardButton.WithCallbackData(
                    $"Email #{c.UserId}",
                    $"{BotCommands.CommandRemindChannelEmail} {c.UserId}"))
                .ToArray();
            rows.Add(chunk);
        }

        return new InlineKeyboardMarkup(rows);
    }
}
