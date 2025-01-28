using System.ComponentModel.DataAnnotations;

namespace DataGateVPNBotV1.Models;

public class IssuedOvpnFile
{
    public int Id { get; set; }

    [Required]
    public long TelegramId { get; set; }
    [Required]
    public string CertName { get; set; } = null!;
    public string CertId { get; set; } = string.Empty;
    public string FileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public DateTime IssuedAt { get; set; }

    public string IssuedTo { get; set; } = null!;
    [Required]
    public string PemFilePath { get; set; } = null!;
    [Required]
    public string CertFilePath { get; set; } = null!;
    [Required]
    public string KeyFilePath { get; set; } = null!;
    [Required]
    public string ReqFilePath { get; set; } = null!;
    [Required]
    public bool IsRevoked { get; set; } = false;
    public string Message { get; set; } = string.Empty;
}
