using DataGateVPNBot.Models.Configurations;
using Telegram.Bot;

namespace DataGateVPNBot.Configurations;

public static class TelegramConfiguration
{
    public static void ConfigureTelegram(this IServiceCollection services, IConfiguration configuration)
    {
        var botConfigSection = configuration.GetSection("BotConfiguration").Get<BotConfiguration>();

        if (botConfigSection == null) throw new NullReferenceException();
        services.AddHttpClient(botConfigSection.TelegramWebHook).AddTypedClient<ITelegramBotClient>(
            httpClient => new TelegramBotClient(botConfigSection.BotToken, httpClient)
        );
    }
}