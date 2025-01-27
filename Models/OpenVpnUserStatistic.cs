using System.ComponentModel.DataAnnotations;

namespace DataGateVPNBotV1.Models;

public class OpenVpnUserStatistic
{
    [Key]
    public int Id { get; set; }

    [Required]
    public Guid SessionId { get; set; }

    [Required]
    [MaxLength(255)]
    public string CommonName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string RealAddress { get; set; } = string.Empty;

    [Required]
    public long BytesReceived { get; set; }

    [Required]
    public long BytesSent { get; set; }

    [Required]
    public DateTime ConnectedSince { get; set; }

    [Required]
    public DateTime LastUpdated { get; set; }
}