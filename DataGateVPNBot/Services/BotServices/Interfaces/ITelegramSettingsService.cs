﻿using DataGateVPNBot.Models.Enums;
using Telegram.Bot.Types;

namespace DataGateVPNBot.Services.BotServices.Interfaces;

public interface ITelegramSettingsService
{
    BotCommand[] GetTelegramMenuByLanguage(Language language);
}