using DataGateVPNBot.Services.BotServices;
using DataGateVPNBot.Services.DashboardServices.Interfaces;
using DataGateMonitor.SharedModels.DataGateMonitor.TelegramBotLocalization.Requests;
using DataGateMonitor.SharedModels.DataGateMonitor.TelegramBotLocalization.Responses;
using Moq;
using Telegram.Bot.Types;
using Xunit;

namespace DataGateVPNBot.Tests.Services;

public class FreeTierAccessComplianceBotServiceTests
{
    [Fact]
    public void IsActiveMember_RecognizesMemberStatus()
    {
        Assert.True(FreeTierAccessComplianceBotService.IsActiveMember(new ChatMemberMember()));
    }

    [Fact]
    public void IsActiveMember_RejectsLeftStatus()
    {
        Assert.False(FreeTierAccessComplianceBotService.IsActiveMember(new ChatMemberLeft()));
    }

    [Theory]
    [InlineData(12345, true, "api/users/audit-free-tier-access/by-telegram/12345?channelSubscribed=true")]
    [InlineData(12345, false, "api/users/audit-free-tier-access/by-telegram/12345?channelSubscribed=false")]
    [InlineData(12345, null, "api/users/audit-free-tier-access/by-telegram/12345")]
    public void BuildAuditEndpoint_FormatsQueryFlag(long telegramId, bool? channelSubscribed, string expected)
    {
        Assert.Equal(expected, FreeTierAccessComplianceBotService.BuildAuditEndpoint(telegramId, channelSubscribed));
    }

    [Fact]
    public async Task BuildAccessDeniedMessageAsync_AppliesLocalizationPlaceholders()
    {
        var localization = new Mock<ILocalizationService>();
        localization.Setup(l => l.GetTextForTelegramUser(
                It.Is<GetTextForTelegramUserRequest>(r =>
                    r.Key == FreeTierAccessComplianceBotService.AccessDeniedLocalizationKey &&
                    r.TelegramId == 42),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetTextForTelegramUserResponse
            {
                Text = "Need {channel}\n{channelUrl}",
            });

        var service = new FreeTierAccessComplianceBotService(
            Mock.Of<Telegram.Bot.ITelegramBotClient>(),
            Microsoft.Extensions.Options.Options.Create(
                new DataGateVPNBot.Models.Configurations.BotConfiguration
                {
                    RequiredChannelUsername = "datagateapp",
                }),
            null!,
            null!,
            localization.Object,
            Mock.Of<Microsoft.Extensions.Logging.ILogger<FreeTierAccessComplianceBotService>>());

        var message = await service.BuildAccessDeniedMessageAsync(42, CancellationToken.None);

        Assert.Equal("Need @datagateapp\nhttps://t.me/datagateapp", message);
        Assert.DoesNotContain("Для тарифа", message);
        Assert.DoesNotContain("Free/Default access requires", message);
    }

    [Theory]
    [InlineData("Bad Request: PARTICIPANT_ID_INVALID", true)]
    [InlineData("Bad Request: user not found", true)]
    [InlineData("Forbidden: bot is not a member of the channel chat", false)]
    public void IsExpectedNonMembershipError_ClassifiesMessages(string message, bool expected)
        => Assert.Equal(
            expected,
            FreeTierAccessComplianceBotService.IsExpectedNonMembershipError(new Exception(message)));

    [Fact]
    public void VpnAccessGateResult_Denied_CarriesUserMessage()
    {
        var result = VpnAccessGateResult.Denied("blocked");

        Assert.False(result.IsAllowed);
        Assert.Equal("blocked", result.UserMessage);
    }
}
