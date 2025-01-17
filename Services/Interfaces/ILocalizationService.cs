using DataGateVPNBotV1.Models.Enums;

namespace DataGateVPNBotV1.Services.Interfaces;

public interface ILocalizationService
{
    Task<string> GetTextAsync(string key, Language language);
}