using DataGateVPNBot.Contexts;
using DataGateVPNBot.Models;
using DataGateVPNBot.Services.DataServices.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataGateVPNBot.Services.DataServices;

public class TelegramUsersService : ITelegramUsersService
{
    private readonly ApplicationDbContext _dbContext;

    public TelegramUsersService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task RegisterUserAsync(long telegramId, string? username, string? firstName, string? lastName)
    {
        var existingUser = await _dbContext.TelegramUsers
            .FirstOrDefaultAsync(u => u.TelegramId == telegramId);

        if (existingUser == null)
        {
            var user = new TelegramUser
            {
                TelegramId = telegramId,
                Username = username,
                FirstName = firstName,
                LastName = lastName
            };

            _dbContext.TelegramUsers.Add(user);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<List<TelegramUser>?> GetAdminsAsync()
    {
        var existingUser = await _dbContext.TelegramUsers
            .Where(u => u.TelegramId == 5767006971).ToListAsync();
        
        return existingUser;
        throw new NotImplementedException();
    }
}
