using DataGateVPNBot.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DataGateVPNBot.Handlers;

public partial class TelegramUpdateHandler
{
    private const string TelegramAlreadyLinkedToGooglePrefix = "TelegramAlreadyLinkedToGoogle|";

    private async Task<Message> CompleteAccountLinkFromBotAsync(
        Message msg,
        string code,
        CancellationToken cancellationToken)
    {
        if (msg.From is null)
            return msg;

        var telegramId = msg.From.Id;
        var result = await authService.CompleteAccountLinkAsync(code, telegramId, cancellationToken);

        string text;
        if (result is { Success: true, Merge: not null } mergeOk)
        {
            text = "✅ " + await GetLocalizationTextAsync(
                "AccountLinkSuccess",
                telegramId,
                new Dictionary<string, string> { ["userId"] = mergeOk.Merge!.SurvivorUserId.ToString() },
                cancellationToken);

            if (mergeOk.Merge.Warnings.Count > 0)
                text += "\n\n⚠️ " + string.Join("\n⚠️ ", mergeOk.Merge.Warnings);
        }
        else if (result is { Success: true })
        {
            text = "✅ " + await GetLocalizationTextAsync("AccountLinkAlreadyLinked", telegramId, cancellationToken);
        }
        else if (result?.Message?.StartsWith(TelegramAlreadyLinkedToGooglePrefix, StringComparison.Ordinal) == true)
        {
            var label = result.Message[TelegramAlreadyLinkedToGooglePrefix.Length..];
            text = "❌ " + await GetLocalizationTextAsync(
                "AccountLinkTelegramAlreadyLinkedToGoogle",
                telegramId,
                new Dictionary<string, string> { ["accountLabel"] = label },
                cancellationToken);
        }
        else if (result?.Message?.Contains("not registered", StringComparison.OrdinalIgnoreCase) == true)
        {
            text = "❌ " + await GetLocalizationTextAsync("AccountLinkNotRegistered", telegramId, cancellationToken);
        }
        else
        {
            text = "❌ " + await GetLocalizationTextAsync("AccountLinkFailed", telegramId, cancellationToken);
        }

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
        if (msg.From is null)
            return msg;

        if (string.IsNullOrWhiteSpace(codeArgument))
        {
            return await _botClient.SendMessage(
                msg.Chat,
                await GetLocalizationTextAsync("AccountLinkEnterCodePrompt", msg.From.Id, cancellationToken),
                cancellationToken: cancellationToken);
        }

        if (!AccountLinkCodeParser.TryNormalizeToken(codeArgument, out var code))
        {
            return await _botClient.SendMessage(
                msg.Chat,
                "❌ " + await GetLocalizationTextAsync("AccountLinkInvalidCodeFormat", msg.From.Id, cancellationToken),
                cancellationToken: cancellationToken);
        }

        return await CompleteAccountLinkFromBotAsync(msg, code, cancellationToken);
    }

    private static bool TryExtractAccountLinkCode(string messageText, out string code)
        => AccountLinkCodeParser.TryExtract(messageText, out code);
}
