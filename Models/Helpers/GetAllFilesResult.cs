namespace DataGateVPNBotV1.Models.Helpers;

public class GetAllFilesResult
{
    public List<FileInfo> FileInfo { get; set; } = null!;
    public string Message { get; set; } = string.Empty;
}