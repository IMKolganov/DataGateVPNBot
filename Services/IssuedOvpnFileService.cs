using DataGateVPNBotV1.Contexts;
using DataGateVPNBotV1.Models;
using DataGateVPNBotV1.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DataGateVPNBotV1.Services;

public class IssuedOvpnFileService : IIssuedOvpnFileService
{
    private readonly ApplicationDbContext _dbContext;

    public IssuedOvpnFileService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddIssuedOvpnFileAsync(long telegramId, FileInfo fileInfo)
    {
        var issuedFile = new IssuedOvpnFile()
        {
            TelegramId = telegramId,
            FileName = fileInfo.Name,
            FilePath = fileInfo.FullName,
            IssuedAt = DateTime.UtcNow,
            IssuedTo = "TgBotUsers",
            CertFilePath = "empty",
            KeyFilePath = "empty",
            PermFilePath = "empty"
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
            .Where(f => f.TelegramId == telegramId)
            .ToListAsync();
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
            existingFile.FileName = issuedFile.FileName;
            existingFile.FilePath = issuedFile.FilePath;
            existingFile.IssuedAt = issuedFile.IssuedAt;
            existingFile.IssuedTo = issuedFile.IssuedTo;

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