namespace DataGateVPNBot.Services.Interfaces;

public interface IErrorService
{
    void LogErrorToDatabase(Exception exception, HttpContext? context = null);
    Task NotifyAdminsAboutExceptionAsync(Exception exception, HttpContext? context = null, CancellationToken cancellationToken = default);
    Task NotifyAdminsAboutStartAsync(CancellationToken cancellationToken = default);
    Task SendMessageToAdminsAsync(string message, CancellationToken cancellationToken);

    /// <summary>
    /// Sends a photo with caption to every bot admin. Falls back to text-only when
    /// <paramref name="photoBytes"/> is null/empty.
    /// </summary>
    Task SendPhotoToAdminsAsync(
        byte[]? photoBytes,
        string caption,
        string fileName = "avatar.jpg",
        CancellationToken cancellationToken = default);
}