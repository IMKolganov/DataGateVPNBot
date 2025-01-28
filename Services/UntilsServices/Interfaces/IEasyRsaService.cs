using DataGateVPNBotV1.Models.Helpers;

namespace DataGateVPNBotV1.Services.UntilsServices.Interfaces;

public interface IEasyRsaService
{
    void InstallEasyRsa();
    CertificateResult BuildCertificate(string certName = "client1");
    string ReadPemContent(string filePath);
    string RevokeCertificate(string clientName);
    List<CertificateCaInfo> FindAllCertificateInfoInIndexFile(string baseFileName);
}