using DataGateVPNBotV1.Models.Enums;
using Telegram.Bot.Types;

namespace DataGateVPNBotV1.Services.BotServices.Interfaces;

public interface ITelegramSettingsService
{
    BotCommand[] GetTelegramMenuByLanguage(Language language);
}