using DataGateVPNBotV1.Models.Helpers;

namespace DataGateVPNBotV1.Services.Interfaces;

public interface IOpenVpnClientService
{
    Task<FileCreationResult> CreateClientConfiguration(string clientName, long telegramId);
}