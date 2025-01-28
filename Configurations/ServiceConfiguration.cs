using DataGateVPNBotV1.Handlers;
using DataGateVPNBotV1.Services;
using DataGateVPNBotV1.Services.BotServices;
using DataGateVPNBotV1.Services.BotServices.Interfaces;
using DataGateVPNBotV1.Services.DataServices;
using DataGateVPNBotV1.Services.DataServices.Interfaces;
using DataGateVPNBotV1.Services.Interfaces;
using DataGateVPNBotV1.Services.UntilsServices;
using DataGateVPNBotV1.Services.UntilsServices.Interfaces;

namespace DataGateVPNBotV1.Configurations;

public static class ServiceConfiguration
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IIssuedOvpnFileService, IssuedOvpnFileService>();
        services.AddScoped<IIncomingMessageLogService, IncomingMessageLogService>();
        services.AddScoped<ITelegramUsersService, TelegramUsersService>();
        services.AddScoped<ILocalizationService, LocalizationService>();
        services.AddScoped<IErrorService, ErrorService>();
        services.AddScoped<IOpenVpnParserService, OpenVpnParserService>();
        services.AddSingleton<TelegramUpdateHandler>();
        services.AddSingleton<ITelegramSettingsService, TelegramSettingsService>();
        services.AddSingleton<IOpenVpnClientService, OpenVpnClientService>();
        services.AddSingleton<IEasyRsaService, EasyRsaService>();
        services.AddHostedService<StartupNotificationHandler>();

        services.AddHostedService<OpenVpnBackgroundService>();
        
        services.ConfigureTelegramBotMvc();

        services.AddControllers();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        
    }
}
