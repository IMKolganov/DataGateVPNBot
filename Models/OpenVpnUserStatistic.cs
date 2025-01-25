namespace DataGateVPNBotV1.Models;

public class OpenVpnUserStatistic
{
    public int Id { get; set; }
    public long TelegramId { get; set; } = 0;
    public string CommonName { get; set; } = string.Empty;
    public string RealAddress { get; set; } = string.Empty;
    public long BytesReceived { get; set; }
    public long BytesSent { get; set; }
    public DateTime ConnectedSince { get; set; }
}