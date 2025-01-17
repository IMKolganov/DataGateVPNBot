using DataGateVPNBotV1.Contexts;
using DataGateVPNBotV1.Models.Enums;
using DataGateVPNBotV1.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataGateVPNBotV1.Services;

public class LocalizationService: ILocalizationService
{
    private readonly ApplicationDbContext _context;

    public LocalizationService(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public async Task<string> GetTextAsync(string key, Language language)
    {
        var text = await _context.LocalizationTexts
            .Where(t => t.Key == key && t.Language == language)
            .Select(t => t.Text)
            .FirstOrDefaultAsync();

        return text ?? $"[Translation missing for key: {key}, language: {language}]";
    }
}