namespace DataGateVPNBot.Models.Helpers;

public class CertificateResult
{
    public string CertificatePath { get; set; } = string.Empty;
    public string KeyPath { get; set; } = string.Empty;
    public string RequestPath { get; set; } = string.Empty;
    public string PemPath { get; set; } = string.Empty;
    public string? CertId { get; set; }
}