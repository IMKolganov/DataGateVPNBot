using DataGateMonitor.SharedModels.Enums;
using Telegram.Bot.Types;

namespace DataGateVPNBot.Services.BotServices.Interfaces;

public interface ITelegramSettingsService
{
    /// <param name="includeAdminCommands">
    /// When false (default), only end-user commands are returned for the public bot menu.
    /// Admin-only commands must be registered with <c>BotCommandScopeChat</c> per admin.
    /// </param>
    BotCommand[] GetTelegramMenuByLanguage(Language language, bool includeAdminCommands = false);
}