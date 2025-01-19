using DataGateVPNBotV1.Models;

namespace DataGateVPNBotV1.Services.Interfaces;

public interface IIssuedOvpnFileService
{
    Task AddIssuedOvpnFileAsync(long telegramId, FileInfo fileInfo, string crtPath, string keyPath, string reqPath,
        string pemPath);
    Task<IssuedOvpnFile?> GetIssuedOvpnFileByIdAsync(int id);
    Task<List<IssuedOvpnFile?>> GetIssuedOvpnFilesByTelegramIdAsync(long telegramId);
    Task<IssuedOvpnFile?> GetIssuedOvpnFilesByTelegramAndFileNameIdAsync(long telegramId, string certName);
    Task SetIsRevokeIssuedOvpnFileByTelegramIdAndCertNameAsync(int id, long telegramId, string certName);
    Task<List<IssuedOvpnFile?>> GetAllIssuedOvpnFilesAsync();
    Task UpdateIssuedOvpnFileAsync(IssuedOvpnFile issuedFile);
    Task DeleteIssuedOvpnFileAsync(int id);
}