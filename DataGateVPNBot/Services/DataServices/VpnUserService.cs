using DataGateVPNBot.DataBase.UnitOfWork;
using DataGateVPNBot.Models;
using DataGateVPNBot.Services.Untils;
using Microsoft.EntityFrameworkCore;

namespace DataGateVPNBot.Services.DataServices;

public class VpnUserService
{
    private readonly IUnitOfWork _unitOfWork;

    public VpnUserService(IUnitOfWork unitOf)
    {
        _unitOfWork = unitOf;
    }

    public async Task<bool> CreateUserAsync(string username, string password, long telegramId)
    {
        var vpnUserRepository = _unitOfWork.GetRepository<VpnUser>();
        var existingVpnUser = await vpnUserRepository.Query
            .FirstOrDefaultAsync(x => x.Username == username);
        
        if (existingVpnUser != null)
            return false;

        //todo: check telegramId
        var hashedPassword = PasswordHasher.HashPassword(password);
        var user = new VpnUser
        {
            Username = username,
            TelegramId = telegramId,
            PasswordHash = hashedPassword
        };

        await vpnUserRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ValidateUserAsync(string username, long telegramId, string password)
    {
        var existingVpnUser = await _unitOfWork.GetQuery<VpnUser>()
            .AsQueryable().FirstOrDefaultAsync(x => x.Username == username && x.TelegramId == telegramId);
        return existingVpnUser != null && PasswordHasher.VerifyPassword(password, existingVpnUser.PasswordHash);
    }
}
