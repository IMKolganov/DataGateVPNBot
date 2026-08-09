namespace DataGateVPNBot.Services.BotServices.Interfaces;

public interface ITelegramProfilePhotoDownloader
{
    /// <summary>
    /// Downloads the largest current profile photo for <paramref name="telegramId"/>.
    /// Returns null when missing or inaccessible.
    /// </summary>
    Task<(byte[] Bytes, string? FileUniqueId)?> TryDownloadAsync(
        long telegramId,
        CancellationToken cancellationToken = default);
}
