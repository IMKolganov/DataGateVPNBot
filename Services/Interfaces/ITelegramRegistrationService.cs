namespace DataGateVPNBotV1.Services;

public interface ITelegramRegistrationService
{
    Task RegisterUserAsync(long telegramId, string? username, string? firstName, string? lastName);
}