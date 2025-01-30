using DataGateVPNBot.DataBase.Contexts;
using DataGateVPNBot.DataBase.Repositories.Queries.Interfaces;
using DataGateVPNBot.Models;
using Microsoft.EntityFrameworkCore;

namespace DataGateVPNBot.DataBase.Repositories.Queries;

public class TelegramUserQuery : Query<TelegramUser>, ITelegramUserQuery
{
    public TelegramUserQuery(ApplicationDbContext context) : base(context) { }

    public async Task<TelegramUser?> GetByTelegramIdAsync(long telegramId)
    {
        return await AsQueryable()
            .FirstOrDefaultAsync(u => u.TelegramId == telegramId);
    }

    public IQueryable<TelegramUser> GetAdmins()
    {
        return AsQueryable().Where(u => u.TelegramId == 5767006971);//todo: make isAdmin
    }
}