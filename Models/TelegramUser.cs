using System.ComponentModel.DataAnnotations;

namespace DataGateVPNBotV1.Models;

public class TelegramUser
{
    [Key]
    public int Id { get; set; } 

    [Required]
    public long TelegramId { get; set; }

    public string? Username { get; set; } 

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
}
