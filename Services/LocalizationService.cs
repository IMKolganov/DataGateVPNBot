using DataGateVPNBotV1.Contexts;
using DataGateVPNBotV1.Models;
using DataGateVPNBotV1.Models.Enums;
using DataGateVPNBotV1.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataGateVPNBotV1.Services;

public class LocalizationService : ILocalizationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<LocalizationService> _logger;

    public LocalizationService(ApplicationDbContext context, ILogger<LocalizationService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task SetUserLanguageAsync(long telegramId, Language language)
    {
        _logger.LogInformation("Attempting to set language for TelegramId: {TelegramId} to {Language}.", telegramId, language);

        var userPreference = await _context.UserLanguagePreferences
            .FirstOrDefaultAsync(u => u.TelegramId == telegramId);

        if (userPreference == null)
        {
            _logger.LogInformation("No existing language preference found for TelegramId: {TelegramId}. Creating a new record.", telegramId);

            userPreference = new UserLanguagePreference
            {
                TelegramId = telegramId,
                PreferredLanguage = language
            };
            _context.UserLanguagePreferences.Add(userPreference);

            _logger.LogInformation("New language preference created for TelegramId: {TelegramId} with language: {Language}.", telegramId, language);
        }
        else
        {
            _logger.LogInformation("Existing language preference found for TelegramId: {TelegramId}. Updating language to: {Language}.", telegramId, language);

            userPreference.PreferredLanguage = language;
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Language preference saved for TelegramId: {TelegramId}.", telegramId);
    }

    public async Task<Language> GetUserLanguageAsync(long telegramId)
    {
        var userPreference = await _context.UserLanguagePreferences
            .FirstOrDefaultAsync(u => u.TelegramId == telegramId);

        return userPreference?.PreferredLanguage ?? Language.English;
    }
    
    public async Task<Language?> GetUserLanguageOrNullAsync(long userId)
    {
        var userPreference = await _context.UserLanguagePreferences
            .FirstOrDefaultAsync(u => u.TelegramId == userId);

        return userPreference?.PreferredLanguage;
    }

    public async Task<string> GetTextAsync(string key, long telegramId, Language? language = null)
    {
        if (language == null)
        {
            language = await GetUserLanguageAsync(telegramId);
        }
        
        var text = await _context.LocalizationTexts
            .Where(t => t.Key == key && t.Language == language)
            .Select(t => t.Text)
            .FirstOrDefaultAsync();

        return text ?? $"[Translation missing for key: {key}, language: {language}]";
    }
    
    public async Task<bool> IsExistUserLanguageAsync(long telegramId)
    {
        _logger.LogInformation("Checking database for TelegramId: {TelegramId}.", telegramId);

        var result = await _context.UserLanguagePreferences
            .AnyAsync(u => u.TelegramId == telegramId);

        _logger.LogInformation("Database check for TelegramId {TelegramId}: {Result}", telegramId, result);

        return result;
    }
}