using DataGateVPNBot.Services.BotServices;
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
    public void BuildAccessDeniedMessage_RequiresChannelSubscriptionOnly()
    {
        var service = new FreeTierAccessComplianceBotService(
            Mock.Of<Telegram.Bot.ITelegramBotClient>(),
            Microsoft.Extensions.Options.Options.Create(
                new DataGateVPNBot.Models.Configurations.BotConfiguration
                {
                    RequiredChannelUsername = "DataGateVPNBot",
                }),
            null!,
            null!,
            Mock.Of<Microsoft.Extensions.Logging.ILogger<FreeTierAccessComplianceBotService>>());

        var message = service.BuildAccessDeniedMessage();

        Assert.Contains("@DataGateVPNBot", message);
        Assert.DoesNotContain("/link_account", message);
        Assert.DoesNotContain("linked account", message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("subscription", message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void VpnAccessGateResult_Denied_CarriesUserMessage()
    {
        var result = VpnAccessGateResult.Denied("blocked");

        Assert.False(result.IsAllowed);
        Assert.Equal("blocked", result.UserMessage);
    }
}
