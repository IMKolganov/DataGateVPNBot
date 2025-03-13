using System.Net.Http.Headers;
using DataGateVPNBot.Models.Helpers.Configurations;
using DataGateVPNBot.Services;
using DataGateVPNBot.Services.DashboardServices;
using StackExchange.Redis;

namespace DataGateVPNBot.Configurations;

public static class DashboardApiConfiguration
{
    public static void ConfigureDashboardApi(this IServiceCollection services, IConfiguration configuration, DashboardApiConfig dashboardApiConfig)
    {
        var redisConfig = configuration.GetSection("Redis").Get<RedisConfig>() ?? new RedisConfig();

        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(redisConfig.ConnectionString));

        services.AddHttpClient("DashboardClient", client =>
        {
            client.BaseAddress = new Uri(dashboardApiConfig.Url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });

        services.AddSingleton<IHttpClientFactoryService, HttpClientFactoryService>();
        services.AddSingleton<RedisCacheService>();

        services.AddSingleton(provider =>
            new DashBoardApiAuthService(
                provider.GetRequiredService<IHttpClientFactoryService>(),
                provider.GetRequiredService<RedisCacheService>(),
                dashboardApiConfig.ClientId,
                dashboardApiConfig.ClientSecret,
                provider.GetRequiredService<ILogger<DashBoardApiAuthService>>()));
    }
}
