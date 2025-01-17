﻿using DataGateVPNBotV1.Models.Enums;

namespace DataGateVPNBotV1.Models;

public class UserLanguagePreference
{
    public int Id { get; set; }
    public int TelegramId { get; set; }
    public Language PreferredLanguage { get; set; }
}