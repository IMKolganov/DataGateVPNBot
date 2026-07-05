using DataGateVPNBot.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DataGateVPNBot.Handlers;

public partial class TelegramUpdateHandler
{
    private async Task<Message> CompleteAccountLinkFromBotAsync(
        Message msg,
        string code,
        CancellationToken cancellationToken)
    {
        if (msg.From is null)
            return msg;

        var telegramId = msg.From.Id;
        var result = await authService.CompleteAccountLinkAsync(code, telegramId, cancellationToken);

        var text = result switch
        {
            { Success: true, Merge: not null } mergeOk =>
                "✅ Аккаунты успешно связаны.\n" +
                "✅ Accounts linked successfully.\n\n" +
                $"User #{mergeOk.Merge!.SurvivorUserId}" +
                (mergeOk.Merge.Warnings.Count > 0
                    ? "\n\n⚠️ " + string.Join("\n⚠️ ", mergeOk.Merge.Warnings)
                    : string.Empty),
            { Success: true } ok =>
                "✅ " + (string.IsNullOrWhiteSpace(ok.Message)
                    ? "Accounts linked successfully."
                    : ok.Message),
            { Message: var message } when message.Contains("not registered", StringComparison.OrdinalIgnoreCase) =>
                "❌ " + message + "\n\nИспользуйте /register в боте.\nUse /register in the bot first.",
            _ =>
                "❌ " + (string.IsNullOrWhiteSpace(result?.Message)
                    ? "Could not link accounts. Check the code and try again."
                    : result!.Message),
        };

        return await _botClient.SendMessage(
            msg.Chat,
            text,
            cancellationToken: cancellationToken);
    }

    private async Task<Message> LinkAccountCommandAsync(
        Message msg,
        string? codeArgument,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(codeArgument))
        {
            return await _botClient.SendMessage(
                msg.Chat,
                "Введите код из приложения:\n/link_account КОД\n\n" +
                "Or send the 8-character code alone in this chat.\n\n" +
                "Enter the code from the app:\n/link_account CODE",
                cancellationToken: cancellationToken);
        }

        if (!AccountLinkCodeParser.TryNormalizeToken(codeArgument, out var code))
        {
            return await _botClient.SendMessage(
                msg.Chat,
                "❌ Неверный формат кода. Нужны 8 символов (A-Z, 2-9).\n" +
                "❌ Invalid code format. Expected 8 characters (A-Z, 2-9).",
                cancellationToken: cancellationToken);
        }

        return await CompleteAccountLinkFromBotAsync(msg, code, cancellationToken);
    }

    private static bool TryExtractAccountLinkCode(string messageText, out string code)
        => AccountLinkCodeParser.TryExtract(messageText, out code);
}
