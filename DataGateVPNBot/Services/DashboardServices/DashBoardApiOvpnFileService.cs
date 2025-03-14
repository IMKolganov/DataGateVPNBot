using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataGateVPNBot.Models.DashBoardApi;
using DataGateVPNBot.Services.Http;
using Microsoft.Extensions.Logging;

namespace DataGateVPNBot.Services.DashboardServices;

public class DashBoardApiOvpnFileService
{
    private readonly ILogger<DashBoardApiOvpnFileService> _logger;
    private readonly IHttpRequestService _httpRequestService;
    private readonly DashBoardApiAuthService _dashBoardApiAuthService;
    private const string Endpoint = "GetAllByExternalIdOvpnFiles";

    public DashBoardApiOvpnFileService(ILogger<DashBoardApiOvpnFileService> logger,
        IHttpRequestService httpRequestService,
        DashBoardApiAuthService dashBoardApiAuthService
        )
    {
        _logger = logger;
        _httpRequestService = httpRequestService;
        _dashBoardApiAuthService = dashBoardApiAuthService;
    }

    public async Task<List<IssuedOvpnFileResponse>?> GetAllOvpnFilesByExternalIdAsync(
        int vpnServerId, string externalId, CancellationToken cancellationToken)
    {
        if (vpnServerId == 0)
        {
            _logger.LogWarning("Invalid request: vpnServerId is required.");
            throw new ArgumentException("vpnServerId is required.");
        }

        if (string.IsNullOrEmpty(externalId))
        {
            _logger.LogWarning("Invalid request: externalId is required.");
            throw new ArgumentException("externalId is required.");
        }
        
        var token = await _dashBoardApiAuthService.GetTokenAsync();
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogError("Failed to retrieve Bearer token.");
            return null;
        }

        var url = $"{Endpoint}?vpnServerId={vpnServerId}&externalId={externalId}";

        _logger.LogInformation($"Requesting OVPN files for Server ID: {vpnServerId}, External ID: {externalId}");
        
        var response = await _httpRequestService.GetAsync<List<IssuedOvpnFileResponse>>(url, token, cancellationToken);

        if (response == null)
        {
            _logger.LogError("Failed to fetch OVPN files from API.");
        }

        return response;
    }

}