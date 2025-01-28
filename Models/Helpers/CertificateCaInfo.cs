namespace DataGateVPNBotV1.Models.Helpers;

public class CertificateCaInfo
{
    public string Status { get; set; } = string.Empty;
    public DateTime? ExpiryDate { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public string UnknownField { get; set; } = string.Empty;
    public string CommonName { get; set; } = string.Empty;
}