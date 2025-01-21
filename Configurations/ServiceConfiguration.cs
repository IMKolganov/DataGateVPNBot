using DataGateVPNBotV1.Contexts;
using DataGateVPNBotV1.Models;
using DataGateVPNBotV1.Services;
using DataGateVPNBotV1.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

namespace DataGateVPNBotV1.Configurations;

public static class ServiceConfiguration
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IIssuedOvpnFileService, IssuedOvpnFileService>();
        services.AddScoped<IIncomingMessageLogService, IncomingMessageLogService>();
        services.AddScoped<ITelegramRegistrationService, TelegramRegistrationService>();
        services.AddScoped<ILocalizationService, LocalizationService>();
        services.AddSingleton<TelegramUpdateHandler>();
        services.AddSingleton<IOpenVpnClientService, OpenVpnClientService>();
        services.AddSingleton<IEasyRsaService, EasyRsaService>();

        services.ConfigureTelegramBotMvc();

        services.AddControllers();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        
    }
}
