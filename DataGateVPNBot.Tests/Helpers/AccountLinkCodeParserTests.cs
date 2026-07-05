using DataGateVPNBot.Helpers;
using Xunit;

namespace DataGateVPNBot.Tests.Helpers;

public class AccountLinkCodeParserTests
{
    [Theory]
    [InlineData("ABCD2345", true, "ABCD2345")]
    [InlineData("abcd2345", true, "ABCD2345")]
    [InlineData("/link_account ABCD2345", false, "")]
    [InlineData("SHORT", false, "")]
    [InlineData("ABCD234!", false, "")]
    [InlineData("12345678", false, "")]
    public void TryExtract_ParsesEightCharacterCodes(string input, bool expected, string expectedCode)
    {
        var ok = AccountLinkCodeParser.TryExtract(input, out var code);

        Assert.Equal(expected, ok);
        Assert.Equal(expectedCode, code);
    }
}
