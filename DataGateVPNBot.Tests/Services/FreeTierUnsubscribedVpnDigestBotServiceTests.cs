using System.Security.Authentication;
using DataGateVPNBot.Services.BotServices;
using DataGateVPNBot.Services.DashboardServices;
using DataGateVPNBot.Services.Http;
using DataGateMonitor.SharedModels.DataGateMonitor.FreeTierEnforcement.Dto;
using DataGateMonitor.SharedModels.DataGateMonitor.FreeTierEnforcement.Enums;
using DataGateMonitor.SharedModels.DataGateMonitor.FreeTierEnforcement.Responses;
using DataGateMonitor.SharedModels.Responses;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DataGateVPNBot.Tests.Services;

public class FreeTierUnsubscribedVpnDigestBotServiceTests
{
    [Fact]
    public async Task GetDigestAsync_ReturnsStructuredData_WhenApiSucceeds()
    {
        var http = new Mock<IHttpRequestService>();
        http.Setup(h => h.PostAsync<ApiResponse<DataGateMonitor.SharedModels.DataGateMonitor.Auth.Responses.TokenResponse>>(
                It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApiResponse<DataGateMonitor.SharedModels.DataGateMonitor.Auth.Responses.TokenResponse>
            {
                Success = true,
                Data = new DataGateMonitor.SharedModels.DataGateMonitor.Auth.Responses.TokenResponse { Token = "t" },
            });
        http.Setup(h => h.GetAsync<ApiResponse<FreeTierUnsubscribedVpnDigestResponse>>(
                "api/free-tier-enforcement/unsubscribed-vpn-digest", "t", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApiResponse<FreeTierUnsubscribedVpnDigestResponse>
            {
                Success = true,
                Data = new FreeTierUnsubscribedVpnDigestResponse
                {
                    Text = "digest-text",
                    Candidates =
                    [
                        new FreeTierEnforcementCandidateDto { UserId = 150, Email = "a@b.c" },
                        new FreeTierEnforcementCandidateDto { UserId = 7, Email = null },
                    ],
                },
            });

        var auth = new AuthService(http.Object, "c", "s", Mock.Of<ILogger<AuthService>>());
        var sut = new FreeTierUnsubscribedVpnDigestBotService(
            auth, http.Object, Mock.Of<ILogger<FreeTierUnsubscribedVpnDigestBotService>>());

        var digest = await sut.GetDigestAsync(CancellationToken.None);

        Assert.NotNull(digest);
        Assert.Equal("digest-text", digest!.Text);
        Assert.Equal(2, digest.Candidates.Count);
        Assert.Equal("a@b.c", digest.Candidates[0].Email);
    }

    [Fact]
    public async Task GetDigestAsync_Throws_WhenNoToken()
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

        await Assert.ThrowsAsync<AuthenticationException>(() => sut.GetDigestAsync(CancellationToken.None));
    }
}

public class FreeTierEmailRemindKeyboardTests
{
    [Fact]
    public void BuildEmailRemindKeyboard_OnlyCandidatesWithEmail()
    {
        var keyboard = DataGateVPNBot.Handlers.TelegramUpdateHandler.BuildEmailRemindKeyboard(
        [
            new FreeTierEnforcementCandidateDto { UserId = 150, DisplayName = "A", Email = "a@x.com" },
            new FreeTierEnforcementCandidateDto { UserId = 7, DisplayName = "B", Email = null },
            new FreeTierEnforcementCandidateDto { UserId = 9, DisplayName = "C", Email = "  " },
            new FreeTierEnforcementCandidateDto { UserId = 3, DisplayName = "D", Email = "d@x.com" },
        ]);

        Assert.NotNull(keyboard);
        var buttons = keyboard!.InlineKeyboard.SelectMany(r => r).ToList();
        Assert.Equal(2, buttons.Count);
        Assert.Contains(buttons, b => b.Text == "Email #150" && b.CallbackData == "/remind_channel_email 150");
        Assert.Contains(buttons, b => b.Text == "Email #3" && b.CallbackData == "/remind_channel_email 3");
    }

    [Fact]
    public void BuildEmailRemindKeyboard_ReturnsNull_WhenNoEmails()
    {
        var keyboard = DataGateVPNBot.Handlers.TelegramUpdateHandler.BuildEmailRemindKeyboard(
        [
            new FreeTierEnforcementCandidateDto { UserId = 1, Email = null },
        ]);

        Assert.Null(keyboard);
    }
}
