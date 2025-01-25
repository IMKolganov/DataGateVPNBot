using Telegram.Bot.Types.Enums;

namespace DataGateVPNBotV1.Models;

public class IncomingMessageLog
{
    public int Id { get; set; }
    public long TelegramId { get; set; }
    public string? Username { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string MessageText { get; set; } = null!;
    public string? FileType { get; set; }
    public string? FileId { get; set; }
    public string? FileName { get; set; }
    public long? FileSize { get; set; }
    public string? FilePath { get; set; }
    public DateTime ReceivedAt { get; set; }
}