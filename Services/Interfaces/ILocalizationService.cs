using DataGateVPNBotV1.Models.Enums;

namespace DataGateVPNBotV1.Services.Interfaces;

public interface ILocalizationService
{
    Task SetUserLanguageAsync(long telegramId, Language language);
    Task<Language> GetUserLanguageAsync(long userId);
    Task<Language?> GetUserLanguageOrNullAsync(long userId);
    Task<string> GetTextAsync(string key, long telegramId, Language? language = null);
    Task<bool> IsExistUserLanguageAsync(long telegramId);
}