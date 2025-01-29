using DataGateVPNBot.Models.Enums;

namespace DataGateVPNBot.Services.DataServices.Interfaces;

public interface ILocalizationService
{
    Task SetUserLanguageAsync(long telegramId, Language language);
    Task<Language> GetUserLanguageAsync(long userId);
    Task<Language?> GetUserLanguageOrNullAsync(long userId);
    Task<string> GetTextAsync(string key, long telegramId, Language? language = null);
    Task<bool> IsExistUserLanguageAsync(long telegramId);
}