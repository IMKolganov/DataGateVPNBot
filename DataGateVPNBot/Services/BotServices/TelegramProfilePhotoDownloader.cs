using DataGateVPNBot.Services.BotServices.Interfaces;
using Telegram.Bot;

namespace DataGateVPNBot.Services.BotServices;

public sealed class TelegramProfilePhotoDownloader(
    ILogger<TelegramProfilePhotoDownloader> logger,
    ITelegramBotClient botClient) : ITelegramProfilePhotoDownloader
{
    public async Task<(byte[] Bytes, string? FileUniqueId)?> TryDownloadAsync(
        long telegramId,
        CancellationToken cancellationToken = default)
    {
        if (telegramId <= 0)
            return null;

        try
        {
            var photos = await botClient.GetUserProfilePhotos(telegramId, offset: 0, limit: 1, cancellationToken);
            if (photos.TotalCount == 0 || photos.Photos.Length == 0)
                return null;

            var sizes = photos.Photos[^1];
            if (sizes.Length == 0)
                return null;

            var biggest = sizes[^1];
            await using var ms = new MemoryStream();
            await botClient.GetInfoAndDownloadFile(biggest.FileId, ms, cancellationToken);
            var bytes = ms.ToArray();
            if (bytes.Length == 0)
                return null;

            var uniqueId = string.IsNullOrEmpty(biggest.FileUniqueId) ? null : biggest.FileUniqueId;
            return (bytes, uniqueId);
        }
        catch (Exception ex) when (TelegramProfilePhotoAccessHelper.IsUserUnavailableForBot(ex))
        {
            logger.LogInformation(
                "No profile photo for TelegramId {Id}: user unavailable ({Message})",
                telegramId, ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to download profile photo for TelegramId {Id}", telegramId);
            return null;
        }
    }
}
