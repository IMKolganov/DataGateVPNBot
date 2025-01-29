using System.Reflection;
using DataGateVPNBot.Services;
using DataGateVPNBot.Services.UntilsServices;

namespace DataGateVPNBot.Configurations;

public static class PipelineConfiguration
{
    public static void ConfigurePipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        
        app.MapGet("/", (ILogger<EasyRsaService> logger) =>
        {
            logger.LogInformation("Hello, Elasticsearch!");
            return "Hello, Elasticsearch!";
        });
        
        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown version";
        var environmentName = app.Environment.EnvironmentName;
        app.Logger.LogInformation("Application version: {Version} ; Environment: {Environment}", version, environmentName);
    }
}