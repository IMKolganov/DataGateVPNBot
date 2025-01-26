using System.Reflection;
using DataGateVPNBotV1.Middlewares;

namespace DataGateVPNBotV1.Configurations;

public static class MiddlewareConfiguration
{
    public static void ConfigureMiddleware(this WebApplication app)
    {
        app.UseMiddleware<GlobalExceptionMiddleware>();
    }
}