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

        return await CompleteAccountLinkFromBotAsync(msg, codeArgument.Trim(), cancellationToken);
    }

    private static bool TryExtractAccountLinkCode(string messageText, out string code)
    {
        code = string.Empty;
        var trimmed = messageText.Trim();
        if (trimmed.StartsWith('/'))
            return false;

        if (trimmed.Length != 8)
            return false;

        foreach (var ch in trimmed)
        {
            if (!IsAccountLinkCodeChar(ch))
                return false;
        }

        code = trimmed.ToUpperInvariant();
        return true;
    }

    private static bool IsAccountLinkCodeChar(char ch)
        => ch is >= 'A' and <= 'Z' or >= '2' and <= '9';
}
