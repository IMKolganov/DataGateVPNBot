using System.Security.Authentication;
using DataGateVPNBot.Services.BotServices.Interfaces;
using DataGateVPNBot.Services.DashboardServices.Interfaces;
using DataGateMonitor.SharedModels.DataGateMonitor.FreeTierEnforcement.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DataGateVPNBot.Handlers;

public partial class TelegramUpdateHandler
{
    private async Task<Message> AdminRemindChannelSubscribeAsync(
        Message msg,
        string? argument,
        CancellationToken cancellationToken)
        => await AdminRemindChannelAsync(
            msg,
            msg.From,
            argument,
            FreeTierChannelSubscribeRemindChannel.Telegram,
            cancellationToken);

    private async Task<Message> AdminRemindChannelEmailAsync(
        Message msg,
        string? argument,
        CancellationToken cancellationToken)
        => await AdminRemindChannelAsync(
            msg,
            msg.From,
            argument,
            FreeTierChannelSubscribeRemindChannel.Email,
            cancellationToken);

    private async Task<Message> AdminRemindChannelEmailFromCallbackAsync(
        Message chatMessage,
        User admin,
        string? argument,
        CancellationToken cancellationToken)
        => await AdminRemindChannelAsync(
            chatMessage,
            admin,
            argument,
            FreeTierChannelSubscribeRemindChannel.Email,
            cancellationToken);

    private async Task<Message> AdminRemindChannelSubscribeFromCallbackAsync(
        Message chatMessage,
        User admin,
        string? argument,
        CancellationToken cancellationToken)
        => await AdminRemindChannelAsync(
            chatMessage,
            admin,
            argument,
            FreeTierChannelSubscribeRemindChannel.Telegram,
            cancellationToken);

    private async Task<Message> AdminRemindChannelAsync(
        Message msg,
        User? admin,
        string? argument,
        FreeTierChannelSubscribeRemindChannel channel,
        CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var tgUserService = scope.ServiceProvider.GetRequiredService<ITelegramBotUserService>();

        var adminId = admin?.Id ?? 0;
        if (adminId <= 0 ||
            !await tgUserService.IsTelegramDashboardAdminAsync(adminId, cancellationToken))
        {
            return await _botClient.SendMessage(
                msg.Chat.Id,
                "⛔ This command is only for bot administrators.",
                cancellationToken: cancellationToken);
        }

        if (string.IsNullOrWhiteSpace(argument))
        {
            var usage = channel == FreeTierChannelSubscribeRemindChannel.Email
                ? "Usage: /remind_channel_email <userId>\nExample: /remind_channel_email 150"
                : "Usage: /remind_channel_subscribe <userId|telegramId>\nExample: /remind_channel_subscribe 22";
            return await _botClient.SendMessage(
                msg.Chat.Id,
                usage,
                cancellationToken: cancellationToken);
        }

        try
        {
            var remindService = scope.ServiceProvider.GetRequiredService<IFreeTierChannelSubscribeRemindBotService>();
            var result = await remindService.RemindAsync(argument, channel, cancellationToken);
            var prefix = result.Success ? "" : "❌ ";
            return await _botClient.SendMessage(
                msg.Chat.Id,
                prefix + result.Message,
                cancellationToken: cancellationToken);
        }
        catch (AuthenticationException ex)
        {
            _logger.LogWarning(ex, "Admin channel-subscribe remind: authentication failed");
            return await _botClient.SendMessage(
                msg.Chat.Id,
                "❌ Dashboard authentication failed. Try again later.",
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Admin channel-subscribe remind failed channel={Channel}", channel);
            return await _botClient.SendMessage(
                msg.Chat.Id,
                "❌ Failed to send reminder. Check backend logs.",
                cancellationToken: cancellationToken);
        }
    }
}
