using DataGateVPNBotV1.Contexts;
using DataGateVPNBotV1.Models;
using Microsoft.EntityFrameworkCore;

namespace DataGateVPNBotV1.Services;

public class TelegramRegistrationService : ITelegramRegistrationService
{
    private readonly ApplicationDbContext _dbContext;

    public TelegramRegistrationService(ApplicationDbContext dbContext)
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
}
