using DataGateVPNBotV1.Contexts;
using DataGateVPNBotV1.Models;
using DataGateVPNBotV1.Services;
using DataGateVPNBotV1.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

namespace DataGateVPNBotV1.Configurations;

public static class ServiceConfiguration
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var botConfigSection = configuration.GetSection("BotConfiguration");
        services.Configure<BotConfiguration>(botConfigSection);

        services.AddHttpClient("TelegramWebHook").AddTypedClient<ITelegramBotClient>(
            httpClient => new TelegramBotClient(botConfigSection.Get<BotConfiguration>()!.BotToken, httpClient)
        );

        services.AddScoped<IIssuedOvpnFileService, IssuedOvpnFileService>();
        services.AddScoped<IIncomingMessageLogService, IncomingMessageLogService>();
        services.AddScoped<ITelegramRegistrationService, TelegramRegistrationService>();
        services.AddScoped<ILocalizationService, LocalizationService>();
        services.AddSingleton<UpdateHandler>();
        services.AddSingleton<IOpenVpnClientService, OpenVpnClientService>();
        services.AddSingleton<IEasyRsaService, EasyRsaService>();

        services.ConfigureTelegramBotMvc();

        services.AddControllers();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        
    }
}
