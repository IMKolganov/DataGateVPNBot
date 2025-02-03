using System.ComponentModel.DataAnnotations;

namespace DataGateVPNBot.Models;

public class VpnUser
{
    [Key]
    public int Id { get; set; }
    [Required]
    public long TelegramId { get; set; }
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}