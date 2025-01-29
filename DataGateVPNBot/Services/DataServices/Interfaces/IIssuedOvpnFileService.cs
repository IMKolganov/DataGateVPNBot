using DataGateVPNBot.Models;
using DataGateVPNBot.Models.Helpers;

namespace DataGateVPNBot.Services.DataServices.Interfaces;

public interface IIssuedOvpnFileService
{
    Task AddIssuedOvpnFileAsync(long telegramId, FileInfo fileInfo, CertificateResult certificateResult);
    Task<IssuedOvpnFile?> GetIssuedOvpnFileByIdAsync(int id);
    Task<List<IssuedOvpnFile>> GetIssuedOvpnFilesByTelegramIdAsync(long telegramId);
    Task<IssuedOvpnFile?> GetIssuedOvpnFilesByTelegramAndFileNameIdAsync(long telegramId, string certName);
    Task SetIsRevokeIssuedOvpnFileByTelegramIdAndCertNameAsync(int id, long telegramId, 
        string revokedFilePath, string certName, string message);
    Task<List<IssuedOvpnFile>> GetAllIssuedOvpnFilesAsync();
    Task UpdateIssuedOvpnFileAsync(IssuedOvpnFile issuedFile);
    Task DeleteIssuedOvpnFileAsync(int id);
}