using DataGateVPNBot.Contexts;
using DataGateVPNBot.Models;
using DataGateVPNBot.Models.Helpers;
using DataGateVPNBot.Services.DataServices.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataGateVPNBot.Services.DataServices;

public class IssuedOvpnFileService : IIssuedOvpnFileService
{
    private readonly ApplicationDbContext _dbContext;

    public IssuedOvpnFileService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddIssuedOvpnFileAsync(long telegramId, FileInfo fileInfo, CertificateResult certificateResult)
    {
        var issuedFile = new IssuedOvpnFile()
        {
            TelegramId = telegramId,
            CertName = Path.GetFileNameWithoutExtension(fileInfo.Name),
            CertId = certificateResult.CertId,
            FileName = fileInfo.Name,
            FilePath = fileInfo.FullName,
            IssuedAt = DateTime.UtcNow,
            IssuedTo = "TgBotUsers",
            CertFilePath = certificateResult.CertificatePath,
            KeyFilePath = certificateResult.KeyPath,
            ReqFilePath = certificateResult.RequestPath,
            PemFilePath = certificateResult.PemPath,
            IsRevoked = false
        };
        
        _dbContext.IssuedOvpnFiles.Add(issuedFile);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IssuedOvpnFile?> GetIssuedOvpnFileByIdAsync(int id)
    {
        return await _dbContext.IssuedOvpnFiles.FindAsync(id);
    }
    public async Task<List<IssuedOvpnFile>> GetIssuedOvpnFilesByTelegramIdAsync(long telegramId)
    {
        return await _dbContext.IssuedOvpnFiles
            .Where(f => f.TelegramId == telegramId && f.IsRevoked == false)
            .ToListAsync();
    }

    public async Task<IssuedOvpnFile?> GetIssuedOvpnFilesByTelegramAndFileNameIdAsync(long telegramId, string fileName)
    {
        return await _dbContext.IssuedOvpnFiles
            .Where(f => f.TelegramId == telegramId && f.FileName == fileName && f.IsRevoked == false)
            .FirstOrDefaultAsync();
    }

    public async Task SetIsRevokeIssuedOvpnFileByTelegramIdAndCertNameAsync(int id, long telegramId, 
        string revokedFilePath, string certName, string message)
    {
        var issuedFile = await _dbContext.IssuedOvpnFiles
            .Where(f => f.Id == id && f.TelegramId == telegramId && f.CertName == certName)
            .FirstOrDefaultAsync();

        if (issuedFile == null)
        {
            throw new Exception("Object is not found.");
        }

        issuedFile.FilePath = revokedFilePath;
        issuedFile.IsRevoked = true;
        issuedFile.Message = message;

        await UpdateIssuedOvpnFileAsync(issuedFile);
    }

    public async Task<List<IssuedOvpnFile>> GetAllIssuedOvpnFilesAsync()
    {
        return await _dbContext.IssuedOvpnFiles.ToListAsync();
    }

    public async Task UpdateIssuedOvpnFileAsync(IssuedOvpnFile issuedFile)
    {
        var existingFile = await _dbContext.IssuedOvpnFiles.FindAsync(issuedFile.Id);
        if (existingFile != null)
        {
            existingFile.TelegramId = issuedFile.TelegramId;
            existingFile.CertName = issuedFile.CertName;
            existingFile.CertId = issuedFile.CertId;
            existingFile.FileName = issuedFile.FileName;
            existingFile.FilePath = issuedFile.FilePath;
            existingFile.IssuedAt = issuedFile.IssuedAt;
            existingFile.IssuedTo = issuedFile.IssuedTo;
            existingFile.CertFilePath = issuedFile.CertFilePath;
            existingFile.KeyFilePath = issuedFile.KeyFilePath;
            existingFile.ReqFilePath = issuedFile.ReqFilePath;
            existingFile.PemFilePath = issuedFile.PemFilePath;
            existingFile.IsRevoked = issuedFile.IsRevoked;

            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task DeleteIssuedOvpnFileAsync(int id)
    {
        var issuedFile = await _dbContext.IssuedOvpnFiles.FindAsync(id);
        if (issuedFile != null)
        {
            _dbContext.IssuedOvpnFiles.Remove(issuedFile);
            await _dbContext.SaveChangesAsync();
        }
    }
}