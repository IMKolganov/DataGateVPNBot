using DataGateVPNBot.Localization;
using Xunit;

namespace DataGateVPNBot.Tests.Localization;

public class ApiErrorMessageMapperTests
{
    [Fact]
    public void TryMap_TelegramAlreadyLinkedToGoogle_extracts_account_label()
    {
        var ok = ApiErrorMessageMapper.TryMap(
            "TelegramAlreadyLinkedToGoogle|koz_nik (25052001kozin@gmail.com)",
            out var mapped);

        Assert.True(ok);
        Assert.Equal(
            ApiErrorMessageMapper.LocalizationAccountLinkTelegramAlreadyLinkedToGoogle,
            mapped.LocalizationKey);
        Assert.True(mapped.IsExpectedBusinessError);
        Assert.Equal("koz_nik (25052001kozin@gmail.com)", mapped.Placeholders!["accountLabel"]);
    }

    [Fact]
    public void TryMap_VpnServerNotAllowedByQuotaPlan_key()
    {
        var ok = ApiErrorMessageMapper.TryMap(
            ApiErrorMessageMapper.VpnServerNotAllowedByQuotaPlanKey,
            out var mapped);

        Assert.True(ok);
        Assert.Equal(ApiErrorMessageMapper.LocalizationVpnServerNotAllowedByQuotaPlan, mapped.LocalizationKey);
        Assert.True(mapped.IsExpectedBusinessError);
        Assert.Null(mapped.Placeholders);
    }

    [Fact]
    public void TryMap_legacy_quota_plan_english_message()
    {
        var ok = ApiErrorMessageMapper.TryMap(
            "VPN server 77 is not allowed for user 293's active quota plan 2.",
            out var mapped);

        Assert.True(ok);
        Assert.Equal(ApiErrorMessageMapper.LocalizationVpnServerNotAllowedByQuotaPlan, mapped.LocalizationKey);
    }

    [Fact]
    public void ExtractPrimaryMessage_from_retried_http_error_notification_body()
    {
        const string raw =
            """
            Failed to complete HTTP request to api/open-vpn-files/add-with-token after 3 attempts.
            Attempt 1: BadRequest - Bad Request
            Response body: {"success":false,"message":"VPN server 77 is not allowed for user 293's active quota plan 2.","data":null}
            Attempt 2: BadRequest - Bad Request
            Response body: {"success":false,"message":"VPN server 77 is not allowed for user 293's active quota plan 2.","data":null}
            Attempt 3: BadRequest - Bad Request
            Response body: {"success":false,"message":"VPN server 77 is not allowed for user 293's active quota plan 2.","data":null}
            """;

        var message = ApiErrorMessageMapper.ExtractPrimaryMessage(raw);

        Assert.Equal("VPN server 77 is not allowed for user 293's active quota plan 2.", message);
        Assert.True(ApiErrorMessageMapper.TryMap(raw, out var mapped));
        Assert.Equal(ApiErrorMessageMapper.LocalizationVpnServerNotAllowedByQuotaPlan, mapped.LocalizationKey);
    }

    [Fact]
    public void ExtractPrimaryMessage_from_account_link_http_error_body()
    {
        const string raw =
            """
            Failed to complete HTTP request to api/users/merge-telegram-google/by-link-code after 3 attempts.
            Attempt 1: BadRequest - Bad Request
            Response body: {"success":false,"message":"TelegramAlreadyLinkedToGoogle|koz_nik (25052001kozin@gmail.com)","data":null}
            """;

        Assert.True(ApiErrorMessageMapper.TryMap(raw, out var mapped));
        Assert.Equal(
            ApiErrorMessageMapper.LocalizationAccountLinkTelegramAlreadyLinkedToGoogle,
            mapped.LocalizationKey);
        Assert.Equal("koz_nik (25052001kozin@gmail.com)", mapped.Placeholders!["accountLabel"]);
    }

    [Fact]
    public void TryMap_returns_false_for_unknown_message()
    {
        Assert.False(ApiErrorMessageMapper.TryMap("Something unexpected exploded", out _));
    }
}
