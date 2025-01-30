namespace DataGateVPNBot.Services.Interfaces;

public interface IErrorService
{
    Task LogErrorToDatabase(Exception exception, HttpContext? context = null);
    Task NotifyAdminsAsync(Exception exception, HttpContext? context = null);
    Task NotifyAdminsAboutStartAsync(CancellationToken cancellationToken = default);
}