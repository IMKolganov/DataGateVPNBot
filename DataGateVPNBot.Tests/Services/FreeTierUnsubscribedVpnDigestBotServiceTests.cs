using System.Security.Authentication;
using DataGateVPNBot.Services.BotServices;
using DataGateVPNBot.Services.DashboardServices;
using DataGateVPNBot.Services.Http;
using DataGateMonitor.SharedModels.Responses;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DataGateVPNBot.Tests.Services;

public class FreeTierUnsubscribedVpnDigestBotServiceTests
{
    [Fact]
    public async Task GetDigestTextAsync_ReturnsData_WhenApiSucceeds()
    {
        var http = new Mock<IHttpRequestService>();
        http.Setup(h => h.PostAsync<ApiResponse<DataGateMonitor.SharedModels.DataGateMonitor.Auth.Responses.TokenResponse>>(
                It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApiResponse<DataGateMonitor.SharedModels.DataGateMonitor.Auth.Responses.TokenResponse>
            {
                Success = true,
                Data = new DataGateMonitor.SharedModels.DataGateMonitor.Auth.Responses.TokenResponse { Token = "t" },
            });
        http.Setup(h => h.GetAsync<ApiResponse<string>>(
                "api/free-tier-enforcement/unsubscribed-vpn-digest", "t", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApiResponse<string> { Success = true, Data = "digest-text" });

        var auth = new AuthService(http.Object, "c", "s", Mock.Of<ILogger<AuthService>>());
        var sut = new FreeTierUnsubscribedVpnDigestBotService(
            auth, http.Object, Mock.Of<ILogger<FreeTierUnsubscribedVpnDigestBotService>>());

        var text = await sut.GetDigestTextAsync(CancellationToken.None);

        Assert.Equal("digest-text", text);
    }

    [Fact]
    public async Task GetDigestTextAsync_Throws_WhenNoToken()
    {
        var http = new Mock<IHttpRequestService>();
        http.Setup(h => h.PostAsync<ApiResponse<DataGateMonitor.SharedModels.DataGateMonitor.Auth.Responses.TokenResponse>>(
                It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApiResponse<DataGateMonitor.SharedModels.DataGateMonitor.Auth.Responses.TokenResponse>
            {
                Success = false,
            });

        var auth = new AuthService(http.Object, "c", "s", Mock.Of<ILogger<AuthService>>());
        var sut = new FreeTierUnsubscribedVpnDigestBotService(
            auth, http.Object, Mock.Of<ILogger<FreeTierUnsubscribedVpnDigestBotService>>());

        await Assert.ThrowsAsync<AuthenticationException>(() => sut.GetDigestTextAsync(CancellationToken.None));
    }
}
