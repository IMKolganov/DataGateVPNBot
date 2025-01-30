using DataGateVPNBot.Models;

namespace DataGateVPNBot.DataBase.Repositories.Interfaces;

public interface ITelegramUserRepository : IRepository<TelegramUser>
{
    Task<TelegramUser?> GetByTelegramIdAsync(long telegramId);
}