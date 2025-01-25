using DataGateVPNBotV1.Contexts;
using DataGateVPNBotV1.Models;
using DataGateVPNBotV1.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataGateVPNBotV1.Services;

public class OpenVpnParserService : IOpenVpnParserService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<OpenVpnParserService> _logger;

    public OpenVpnParserService(ApplicationDbContext dbContext, ILogger<OpenVpnParserService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task ParseAndSaveAsync(string statusFilePath)
    {
        if (!File.Exists(statusFilePath))
        {
            _logger.LogError("OpenVPN status file not found: {FilePath}", statusFilePath);
            throw new FileNotFoundException($"OpenVPN status file not found: {statusFilePath}");
        }

        _logger.LogInformation("Started reading the OpenVPN status file: {FilePath}", statusFilePath);
        var users = ParseOpenVpnStatus(statusFilePath);

        _logger.LogInformation("Found {UserCount} users in the status file.", users.Count);

        _logger.LogInformation("Started saving data to the database.");
        foreach (var user in users)
        {
            try
            {
                var existingUser = await _dbContext.OpenVpnUserStatistics
                    .FirstOrDefaultAsync(u => u.CommonName == user.CommonName && u.RealAddress == user.RealAddress);

                if (existingUser != null)
                {
                    _logger.LogDebug("Updating data for user: {CommonName}, address: {RealAddress}",
                        user.CommonName, user.RealAddress);

                    existingUser.BytesReceived = user.BytesReceived;
                    existingUser.BytesSent = user.BytesSent;
                    existingUser.ConnectedSince = user.ConnectedSince;
                }
                else
                {
                    _logger.LogDebug("Adding new user: {CommonName}, address: {RealAddress}",
                        user.CommonName, user.RealAddress);

                    await _dbContext.OpenVpnUserStatistics.AddAsync(user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing user data: {CommonName}, address: {RealAddress}",
                    user.CommonName, user.RealAddress);
            }
        }

        try
        {
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Data successfully saved to the database.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while saving data to the database.");
            throw;
        }
    }

    private List<OpenVpnUserStatistic> ParseOpenVpnStatus(string filePath)
    {
        var users = new List<OpenVpnUserStatistic>();

        foreach (var line in File.ReadAllLines(filePath))
        {
            if (line.StartsWith("CLIENT_LIST"))
            {
                var parts = line.Split(',');
                if (parts.Length >= 5)
                {
                    users.Add(new OpenVpnUserStatistic
                    {
                        TelegramId = 0,
                        CommonName = parts[1],
                        RealAddress = parts[2],
                        BytesReceived = long.Parse(parts[3]),
                        BytesSent = long.Parse(parts[4]),
                        ConnectedSince = DateTime.Parse(parts[5])
                    });
                }
            }
        }

        return users;
    }
}