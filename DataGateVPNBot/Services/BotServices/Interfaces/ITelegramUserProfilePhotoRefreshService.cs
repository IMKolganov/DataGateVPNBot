namespace DataGateVPNBot.Services.BotServices.Interfaces;

public interface ITelegramUserProfilePhotoRefreshService
{
    /// <summary>
    /// For every Telegram bot user in the dashboard DB, fetches the current profile photo from Telegram
    /// and upserts it via <c>POST api/tgbot-users/profile-photo</c>.
    /// </summary>
    Task<ProfilePhotoBatchRefreshResult> RefreshAllFromTelegramAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Downloads the current profile photo for a single Telegram user (largest size).
    /// Returns null when the user has no photo or Telegram refuses access. Optionally upserts to the dashboard.
    /// </summary>
    Task<byte[]?> TryDownloadProfilePhotoAsync(
        long telegramId,
        bool upsertToDashboard = false,
        CancellationToken cancellationToken = default);
}
