using System.Net.Http.Headers;
using DataGateVPNBot.Models.Helpers.Configurations;
using DataGateVPNBot.Services;
using DataGateVPNBot.Services.DashboardServices;

namespace DataGateVPNBot.Configurations;

public static class DashboardApiConfiguration
{
    public static void ConfigureDashboardApi(this IServiceCollection services, DashboardApiConfig dashboardApiConfig)
    {
        services.AddHttpClient("DashboardClient", client =>
        {
            client.BaseAddress = new Uri(dashboardApiConfig.Url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });

        services.AddSingleton<IHttpClientFactoryService, HttpClientFactoryService>();

        services.AddSingleton<DashBoardAuthService>(provider =>
            new DashBoardAuthService(
                provider.GetRequiredService<IHttpClientFactoryService>(),
                dashboardApiConfig.ClientId,
                dashboardApiConfig.ClientSecret,
                provider.GetRequiredService<ILogger<DashBoardAuthService>>()));
    }
}
