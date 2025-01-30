using DataGateVPNBot.DataBase.Repositories.Interfaces;
using DataGateVPNBot.Models;

namespace DataGateVPNBot.DataBase.Repositories.Queries.Interfaces;

public interface ITelegramUserQuery : IQuery<TelegramUser>
{
    Task<TelegramUser?> GetByTelegramIdAsync(long telegramId);
    IQueryable<TelegramUser> GetAdmins();
}