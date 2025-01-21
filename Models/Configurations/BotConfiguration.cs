﻿namespace DataGateVPNBotV1.Models.Configurations;

public class BotConfiguration
{
    public string BotToken { get; init; } = null!;
    public Uri BotWebhookUrl { get; init; } = null!;
    public string SecretToken { get; init; } = null!;
    public string TelegramWebHook { get; init; } = "TelegramWebHook";
    
}