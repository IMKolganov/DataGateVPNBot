using System.Security.Authentication;
using DataGateVPNBot.Localization;
using DataGateVPNBot.Models.Configurations;
using DataGateVPNBot.Services.BotServices.Interfaces;
using DataGateVPNBot.Services.DashboardServices;
using DataGateVPNBot.Services.DashboardServices.Interfaces;
using DataGateVPNBot.Services.Http;
using DataGateMonitor.SharedModels.DataGateMonitor.TelegramBotLocalization.Requests;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DataGateVPNBot.Services.BotServices;

public sealed class FreeTierAccessComplianceBotService(
    ITelegramBotClient botClient,
    IOptions<BotConfiguration> botOptions,
    AuthService authService,
    IHttpRequestService httpRequestService,
    ILocalizationService localizationService,
    ILogger<FreeTierAccessComplianceBotService> logger) : IFreeTierAccessComplianceBotService
{
    public const string AccessDeniedLocalizationKey = "FreeTierAccessDenied";

    private const string AuditEndpointPrefix = "api/users/audit-free-tier-access/by-telegram/";

    public static string BuildAuditEndpoint(long telegramId, bool? channelSubscribed, string? context = null)
    {
        var endpoint = channelSubscribed switch
        {
            true => $"{AuditEndpointPrefix}{telegramId}?channelSubscribed=true",
            false => $"{AuditEndpointPrefix}{telegramId}?channelSubscribed=false",
            _ => $"{AuditEndpointPrefix}{telegramId}",
        };

        if (string.IsNullOrWhiteSpace(context))
            return endpoint;

        var separator = endpoint.Contains('?', StringComparison.Ordinal) ? '&' : '?';
        return $"{endpoint}{separator}context={Uri.EscapeDataString(context)}";
    }

    public async Task<string> BuildAccessDeniedMessageAsync(long telegramId, CancellationToken cancellationToken)
    {
        var channel = botOptions.Value.RequiredChannelChatId;
        var channelUrl = botOptions.Value.RequiredChannelUrl;
        var response = await localizationService.GetTextForTelegramUser(
            new GetTextForTelegramUserRequest
            {
                TelegramId = telegramId > 0 ? telegramId : 0,
                Key = AccessDeniedLocalizationKey,
            },
            cancellationToken);

        var template = response.Text;
        return LocalizationPlaceholderFormatter.Apply(
            template,
            new Dictionary<string, string>
            {
                ["channel"] = channel,
                ["channelUrl"] = channelUrl,
            });
    }

    public async Task<bool?> IsSubscribedToRequiredChannelAsync(long telegramId, CancellationToken cancellationToken)
    {
        if (telegramId <= 0)
            return false;

        try
        {
            var chatId = new ChatId(botOptions.Value.RequiredChannelChatId);
            var member = await botClient.GetChatMember(chatId, telegramId, cancellationToken);
            return IsActiveMember(member);
        }
        catch (Exception ex) when (IsExpectedNonMembershipError(ex))
        {
            logger.LogDebug(
                ex,
                "Telegram user {TelegramId} is not a member of {Channel} (expected getChatMember response)",
                telegramId,
                botOptions.Value.RequiredChannelChatId);
            return false;
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "Failed to verify channel subscription for Telegram user {TelegramId} in {Channel}",
                telegramId,
                botOptions.Value.RequiredChannelChatId);
            return null;
        }
    }

    public static bool IsExpectedNonMembershipError(Exception ex)
    {
        var text = ex.Message;
        if (string.IsNullOrWhiteSpace(text))
            return false;

        var lower = text.ToLowerInvariant();
        return lower.Contains("participant_id_invalid", StringComparison.Ordinal)
               || lower.Contains("user not found", StringComparison.Ordinal)
               || lower.Contains("member not found", StringComparison.Ordinal)
               || lower.Contains("user is deactivated", StringComparison.Ordinal)
               || lower.Contains("user_deactivated", StringComparison.Ordinal)
               || lower.Contains("peer_id_invalid", StringComparison.Ordinal);
    }

    public async Task<VpnAccessGateResult> EnsureVpnAccessAsync(
        long telegramId,
        string context,
        CancellationToken cancellationToken)
    {
        if (telegramId <= 0)
            return VpnAccessGateResult.Denied(await BuildAccessDeniedMessageAsync(0, cancellationToken));

        var channelSubscribed = await IsSubscribedToRequiredChannelAsync(telegramId, cancellationToken);
        if (channelSubscribed == true)
            return VpnAccessGateResult.Allowed;

        var token = await authService.GetTokenAsync();
        if (string.IsNullOrEmpty(token))
            throw new AuthenticationException("Authentication failed. Failed to obtain a valid token from API.");

        var endpoint = BuildAuditEndpoint(telegramId, channelSubscribed, context);

        logger.LogInformation(
            "Checking VPN access for TelegramId={TelegramId}, ChannelSubscribed={ChannelSubscribed}, Context={Context}",
            telegramId,
            channelSubscribed,
            context);

        var response = await httpRequestService.PostAsync<DataGateMonitor.SharedModels.Responses.ApiResponse<bool>>(
            endpoint,
            new { },
            token,
            cancellationToken);

        if (response is { Success: true, Data: true })
            return VpnAccessGateResult.Allowed;

        if (response is not { Success: true })
        {
            logger.LogWarning(
                "VPN access audit request failed for TelegramId={TelegramId}: {Message}",
                telegramId,
                response?.Message);
        }

        return VpnAccessGateResult.Denied(await BuildAccessDeniedMessageAsync(telegramId, cancellationToken));
    }

    public static bool IsActiveMember(ChatMember member)
        => member.Status switch
        {
            ChatMemberStatus.Creator or ChatMemberStatus.Administrator or ChatMemberStatus.Member
                or ChatMemberStatus.Restricted => true,
            _ => false,
        };
}
