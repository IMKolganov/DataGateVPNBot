namespace DataGateVPNBot.Services.BotServices;

public sealed class VpnAccessGateResult
{
    public bool IsAllowed { get; init; }

    public string? UserMessage { get; init; }

    public static VpnAccessGateResult Allowed { get; } = new() { IsAllowed = true };

    public static VpnAccessGateResult Denied(string userMessage) =>
        new() { IsAllowed = false, UserMessage = userMessage };
}
