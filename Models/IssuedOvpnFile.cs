using System.ComponentModel.DataAnnotations;

namespace DataGateVPNBotV1.Models;

public class IssuedOvpnFile
{
    public int Id { get; set; }
    [Required]
    public long TelegramId { get; set; }
    public string FileName { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public DateTime IssuedAt { get; set; }
    public string IssuedTo { get; set; } = null!;
}