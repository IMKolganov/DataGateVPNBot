using DataGateVPNBot.Models.Enums;

namespace DataGateVPNBot.Models;

public class UserLanguagePreference
{
    public int Id { get; set; }
    public long TelegramId { get; set; }
    public Language PreferredLanguage { get; set; }
}