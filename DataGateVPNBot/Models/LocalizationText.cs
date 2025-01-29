using DataGateVPNBot.Models.Enums;

namespace DataGateVPNBot.Models;

public class LocalizationText
{
    public int Id { get; set; }
    public string Key { get; set; } = null!;
    public Language Language { get; set; }
    public string Text { get; set; } = null!;
}