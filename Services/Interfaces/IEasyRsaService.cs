namespace DataGateVPNBotV1.Services.Interfaces;

public interface IEasyRsaService
{
    void InstallEasyRsa();
    void BuildCertificate(string certName = "client1");
    string ReadPemContent(string filePath);
    bool RevokeCertificate(string clientName);
}