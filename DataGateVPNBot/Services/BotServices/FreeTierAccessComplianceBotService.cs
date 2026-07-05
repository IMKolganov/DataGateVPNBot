using System.Security.Authentication;
using DataGateVPNBot.Models.Configurations;
using DataGateVPNBot.Services.BotServices.Interfaces;
using DataGateVPNBot.Services.DashboardServices;
using DataGateVPNBot.Services.Http;
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
    ILogger<FreeTierAccessComplianceBotService> logger) : IFreeTierAccessComplianceBotService
{
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

    public string BuildChannelSubscriptionRequiredMessage()
    {
        var channel = botOptions.Value.RequiredChannelChatId;
        return $"У вас не обнаружена подписка на канал {channel}.\n" +
               $"Подпишитесь на канал и попробуйте снова.\n\n" +
               $"Channel subscription to {channel} was not detected.\n" +
               $"Please subscribe and try again.";
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

    public async Task<VpnAccessGateResult> EnsureVpnAccessAsync(
        long telegramId,
        string context,
        CancellationToken cancellationToken)
    {
        if (telegramId <= 0)
            return VpnAccessGateResult.Denied(BuildChannelSubscriptionRequiredMessage());

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

        return VpnAccessGateResult.Denied(BuildChannelSubscriptionRequiredMessage());
    }

    public static bool IsActiveMember(ChatMember member)
        => member.Status switch
        {
            ChatMemberStatus.Creator or ChatMemberStatus.Administrator or ChatMemberStatus.Member
                or ChatMemberStatus.Restricted => true,
            _ => false,
        };
}
