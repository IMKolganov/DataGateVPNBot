using System.Security.Cryptography.X509Certificates;

namespace DataGateVPNBotV1.Configurations;

public static class WebHostConfiguration
{
    public static void ConfigureWebHost(this WebApplicationBuilder builder)
    {
        var certificate = X509Certificate2.CreateFromPemFile("datagatetgbot.pem", "datagatetgbot.key");//todo: move to config

        builder.WebHost.UseUrls("http://localhost:8443", "https://localhost:8443");//todo: move to config
        builder.WebHost.UseKestrel(options =>
        {
            options.ListenAnyIP(8443, listenOptions =>
            {
                listenOptions.UseHttps(certificate);
            });
        });
    }
}