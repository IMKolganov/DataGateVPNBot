namespace DataGateVPNBot.Helpers;

public static class AccountLinkCodeParser
{
    public static bool TryExtract(string messageText, out string code)
    {
        code = string.Empty;
        var trimmed = messageText.Trim();
        if (trimmed.StartsWith('/'))
            return false;

        if (trimmed.Length != 8)
            return false;

        foreach (var ch in trimmed)
        {
            if (!IsCodeChar(ch))
                return false;
        }

        code = trimmed.ToUpperInvariant();
        return true;
    }

    public static bool IsCodeChar(char ch)
        => ch is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or >= '2' and <= '9';
}
