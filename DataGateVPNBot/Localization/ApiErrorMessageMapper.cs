using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace DataGateVPNBot.Localization;

/// <summary>
/// Maps dashboard API machine messages (and legacy English strings) to LocalizationTexts keys.
/// </summary>
public static partial class ApiErrorMessageMapper
{
    public const string TelegramAlreadyLinkedToGooglePrefix = "TelegramAlreadyLinkedToGoogle|";
    public const string VpnServerNotAllowedByQuotaPlanKey = "VpnServerNotAllowedByQuotaPlan";

    public const string LocalizationAccountLinkTelegramAlreadyLinkedToGoogle =
        "AccountLinkTelegramAlreadyLinkedToGoogle";

    public const string LocalizationVpnServerNotAllowedByQuotaPlan = "VpnServerNotAllowedByQuotaPlan";

    public sealed record MappedError(
        string LocalizationKey,
        IReadOnlyDictionary<string, string>? Placeholders,
        bool IsExpectedBusinessError);

    public static bool TryMap(string? apiOrExceptionMessage, out MappedError mapped)
    {
        mapped = null!;
        var message = ExtractPrimaryMessage(apiOrExceptionMessage);
        if (string.IsNullOrWhiteSpace(message))
            return false;

        if (message.StartsWith(TelegramAlreadyLinkedToGooglePrefix, StringComparison.Ordinal))
        {
            var label = message[TelegramAlreadyLinkedToGooglePrefix.Length..];
            mapped = new MappedError(
                LocalizationAccountLinkTelegramAlreadyLinkedToGoogle,
                new Dictionary<string, string> { ["accountLabel"] = label },
                IsExpectedBusinessError: true);
            return true;
        }

        if (IsVpnServerNotAllowedByQuotaPlan(message))
        {
            mapped = new MappedError(
                LocalizationVpnServerNotAllowedByQuotaPlan,
                Placeholders: null,
                IsExpectedBusinessError: true);
            return true;
        }

        return false;
    }

    public static string ExtractPrimaryMessage(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return string.Empty;

        foreach (Match match in ResponseBodyRegex().Matches(raw))
        {
            var json = match.Groups[1].Value.Trim();
            if (TryReadMessageFromJson(json, out var fromJson))
                return fromJson;
        }

        var trimmed = raw.Trim();
        if (trimmed.StartsWith('{') && TryReadMessageFromJson(trimmed, out var direct))
            return direct;

        return FirstMeaningfulLine(trimmed);
    }

    private static bool IsVpnServerNotAllowedByQuotaPlan(string message)
    {
        if (string.Equals(message, VpnServerNotAllowedByQuotaPlanKey, StringComparison.Ordinal))
            return true;

        if (message.Contains("Access to this VPN server is denied for your quota plan", StringComparison.OrdinalIgnoreCase))
            return true;

        return message.Contains("not allowed", StringComparison.OrdinalIgnoreCase)
               && message.Contains("active quota plan", StringComparison.OrdinalIgnoreCase);
    }

    private static bool TryReadMessageFromJson(string json, out string message)
    {
        message = string.Empty;
        try
        {
            var token = JToken.Parse(json);
            var value = token["message"]?.ToString();
            if (string.IsNullOrWhiteSpace(value))
                return false;
            message = value.Trim();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string FirstMeaningfulLine(string text)
    {
        using var reader = new StringReader(text);
        while (reader.ReadLine() is { } line)
        {
            var trimmed = line.Trim();
            if (trimmed.Length == 0)
                continue;
            if (trimmed.StartsWith("Failed to complete HTTP request", StringComparison.OrdinalIgnoreCase))
                continue;
            if (trimmed.StartsWith("Attempt ", StringComparison.OrdinalIgnoreCase))
                continue;
            if (trimmed.StartsWith("Response body:", StringComparison.OrdinalIgnoreCase))
                continue;
            return trimmed;
        }

        return text.Trim();
    }

    [GeneratedRegex(@"Response body:\s*(\{.*?\})(?=\r?\n|$)", RegexOptions.Singleline)]
    private static partial Regex ResponseBodyRegex();
}
