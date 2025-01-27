using System.Security.Cryptography;
using System.Text;
using DataGateVPNBotV1.Contexts;
using DataGateVPNBotV1.Models;
using DataGateVPNBotV1.Models.Helpers;
using DataGateVPNBotV1.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataGateVPNBotV1.Services;

public class OpenVpnParserService : IOpenVpnParserService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<IOpenVpnParserService> _logger;
    private readonly string _statusFilePath;

    public OpenVpnParserService(ApplicationDbContext dbContext, ILogger<IOpenVpnParserService> logger,
        IConfiguration configuration)
    {
        _dbContext = dbContext;
        _logger = logger;
        _statusFilePath = configuration.GetSection("OpenVpn").Get<OpenVpnSettings>()?.StatusFilePath
                          ?? throw new InvalidOperationException("Failed to load OpenVpnSettings from configuration.");
    }

    public async Task ParseAndSaveAsync()
    {
        if (!File.Exists(_statusFilePath))
        {
            _logger.LogError("OpenVPN status file not found: {FilePath}", _statusFilePath);
            throw new FileNotFoundException($"OpenVPN status file not found: {_statusFilePath}");
        }

        _logger.LogInformation("Started reading the OpenVPN status file: {FilePath}", _statusFilePath);
        var users = ParseOpenVpnStatus(_statusFilePath);

        _logger.LogInformation("Found {UserCount} users in the status file.", users.Count);

        foreach (var user in users)
        {
            var sessionId = GenerateSessionId(user.CommonName, user.RealAddress, user.ConnectedSince);

            var existingUser =
                await _dbContext.OpenVpnUserStatistics.FirstOrDefaultAsync(u => u.SessionId == sessionId);//todo: make service

            if (existingUser != null)
            {
                existingUser.BytesReceived = user.BytesReceived;
                existingUser.BytesSent = user.BytesSent;
                existingUser.LastUpdated = DateTime.UtcNow;
            }
            else
            {
                _dbContext.OpenVpnUserStatistics.Add(new OpenVpnUserStatistic
                {
                    SessionId = sessionId,
                    CommonName = user.CommonName,
                    RealAddress = user.RealAddress,
                    BytesReceived = user.BytesReceived,
                    BytesSent = user.BytesSent,
                    ConnectedSince = user.ConnectedSince,
                    LastUpdated = DateTime.UtcNow
                });
            }
        }

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Data successfully saved to the database.");
    }

    private Guid GenerateSessionId(string commonName, string realAddress, DateTime connectedSince)
    {
        var sessionString = $"{commonName}-{realAddress}-{connectedSince:o}";
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(sessionString));
        return new Guid(hashBytes.Take(16).ToArray());
    }

    private List<OpenVpnUserStatistic> ParseOpenVpnStatus(string filePath)
    {
        var users = new List<OpenVpnUserStatistic>();

        foreach (var line in File.ReadAllLines(filePath))
        {
            if (line.StartsWith("CLIENT_LIST"))
            {
                var parts = line.Split(',');
                if (parts.Length >= 6)
                {
                    users.Add(new OpenVpnUserStatistic
                    {
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
