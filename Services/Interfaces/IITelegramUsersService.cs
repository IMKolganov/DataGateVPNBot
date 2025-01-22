using DataGateVPNBotV1.Models;

namespace DataGateVPNBotV1.Services.Interfaces;

public interface ITelegramUsersService
{
    Task RegisterUserAsync(long telegramId, string? username, string? firstName, string? lastName);
    Task<List<TelegramUser>> GetAdminsAsync();
}