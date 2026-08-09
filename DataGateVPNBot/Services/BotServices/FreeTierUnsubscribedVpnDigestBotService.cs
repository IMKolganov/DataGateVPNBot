using System.Security.Authentication;
using DataGateVPNBot.Services.BotServices.Interfaces;
using DataGateVPNBot.Services.DashboardServices;
using DataGateVPNBot.Services.Http;
using DataGateMonitor.SharedModels.Responses;

namespace DataGateVPNBot.Services.BotServices;

public sealed class FreeTierUnsubscribedVpnDigestBotService(
    AuthService authService,
    IHttpRequestService httpRequestService,
    ILogger<FreeTierUnsubscribedVpnDigestBotService> logger) : IFreeTierUnsubscribedVpnDigestBotService
{
    private const string Endpoint = "api/free-tier-enforcement/unsubscribed-vpn-digest";

    public async Task<string?> GetDigestTextAsync(CancellationToken cancellationToken)
    {
        var token = await authService.GetTokenAsync();
        if (string.IsNullOrEmpty(token))
            throw new AuthenticationException("Authentication failed. Failed to obtain a valid token from API.");

        var response = await httpRequestService.GetAsync<ApiResponse<string>>(Endpoint, token, cancellationToken);
        if (response is { Success: true, Data: not null })
            return response.Data;

        logger.LogWarning(
            "Unsubscribed VPN digest API failed: {Message}",
            response?.Message ?? "null response");
        return null;
    }
}
