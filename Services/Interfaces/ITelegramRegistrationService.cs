namespace DataGateVPNBotV1.Services.Interfaces;

public interface ITelegramRegistrationService
{
    Task RegisterUserAsync(long telegramId, string? username, string? firstName, string? lastName);
}