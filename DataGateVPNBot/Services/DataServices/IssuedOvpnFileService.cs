using DataGateVPNBot.DataBase.Contexts;
using DataGateVPNBot.DataBase.UnitOfWork;
using DataGateVPNBot.Models;
using DataGateVPNBot.Models.Helpers;
using DataGateVPNBot.Services.DataServices.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataGateVPNBot.Services.DataServices;

public class IssuedOvpnFileService : IIssuedOvpnFileService
{
    private readonly IUnitOfWork _unitOfWork;
    public IssuedOvpnFileService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task AddIssuedOvpnFileAsync(long telegramId, FileInfo fileInfo, CertificateResult certificateResult)
    {
        var repository = _unitOfWork.GetRepository<IssuedOvpnFile>();
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
        
        await repository.AddAsync(issuedFile);
        await _unitOfWork.SaveChangesAsync();
    }
    
    public async Task<List<IssuedOvpnFile>> GetIssuedOvpnFilesByTelegramIdAsync(long telegramId)
    {
        return await _unitOfWork.GetQuery<IssuedOvpnFile>()
            .AsQueryable().Where(x => 
                x.TelegramId == telegramId && x.IsRevoked == false)
            .ToListAsync();
    }

    public async Task<IssuedOvpnFile?> GetIssuedOvpnFilesByTelegramAndFileNameIdAsync(long telegramId, string fileName)
    {
        return await _unitOfWork.GetQuery<IssuedOvpnFile>()
        .AsQueryable().Where(x => 
            x.TelegramId == telegramId && x.FileName == fileName && x.IsRevoked == false)
        .FirstOrDefaultAsync();
    }

    public async Task SetIsRevokeIssuedOvpnFileByTelegramIdAndCertNameAsync(int id, long telegramId, 
        string revokedFilePath, string certName, string message)
    {
        var issuedOvpnFileRepository = _unitOfWork.GetRepository<IssuedOvpnFile>();
        var issuedFile = await issuedOvpnFileRepository.Query
            .Where(x => x.Id == id && x.TelegramId == telegramId && x.CertName == certName)
            .FirstOrDefaultAsync();

        if (issuedFile == null)
        {
            throw new Exception("Object is not found.");
        }

        issuedFile.FilePath = revokedFilePath;
        issuedFile.IsRevoked = true;
        issuedFile.Message = message;

        
        issuedOvpnFileRepository.Update(issuedFile);
        await _unitOfWork.SaveChangesAsync();
    }
}