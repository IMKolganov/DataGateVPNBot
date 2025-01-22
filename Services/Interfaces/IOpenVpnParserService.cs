namespace DataGateVPNBotV1.Services.Interfaces;

public interface IOpenVpnParserService
{
    Task ParseAndSaveAsync(string statusFilePath);
}