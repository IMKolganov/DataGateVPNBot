namespace DataGateVPNBotV1.Models;

public class ErrorLog
{
    public int Id { get; set; }
    public string Message { get; set; }
    public string StackTrace { get; set; }
    public DateTime Timestamp { get; set; }
    public string Source { get; set; }
}