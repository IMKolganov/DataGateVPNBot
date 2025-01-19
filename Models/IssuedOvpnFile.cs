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

    // Поля для трёх файлов: Key, Perm и Cert
    // [Required]
    public string KeyFilePath { get; set; } //= null!; // Путь к ключу (Key)

    // [Required]
    public string PermFilePath { get; set; } //= null!; // Путь к файлу прав (Perm)

    // [Required]
    public string CertFilePath { get; set; } //= null!; // Путь к сертификату (Cert)
}
