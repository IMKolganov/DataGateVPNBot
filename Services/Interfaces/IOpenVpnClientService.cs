using DataGateVPNBotV1.Models.Helpers;

namespace DataGateVPNBotV1.Services.Interfaces;

public interface IOpenVpnClientService
{
    Task<GetAllFilesResult> GetAllClientConfigurations(long telegramId);
    Task<FileCreationResult> CreateClientConfiguration(long telegramId);
    Task DeleteAllClientConfigurations(long telegramId);
    Task DeleteClientConfiguration(long telegramId, string filename);
}