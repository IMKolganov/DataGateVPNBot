namespace DataGateVPNBotV1.Services.Interfaces;

public interface IOpenVpnClientService
{
    FileInfo CreateClientConfiguration(string clientName, string serverIp);
}