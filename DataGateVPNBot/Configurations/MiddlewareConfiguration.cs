using System.Reflection;
using DataGateVPNBot.Middlewares;

namespace DataGateVPNBot.Configurations;

public static class MiddlewareConfiguration
{
    public static void ConfigureMiddleware(this WebApplication app)
    {
        app.UseMiddleware<GlobalExceptionMiddleware>();
    }
}