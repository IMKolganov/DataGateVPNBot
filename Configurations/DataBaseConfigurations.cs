using DataGateVPNBotV1.Contexts;
using DataGateVPNBotV1.Models.Helpers;
using Microsoft.EntityFrameworkCore;

namespace DataGateVPNBotV1.Configurations;

public static class DataBaseConfigurations
{
    public static void DataBaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        var dbSettings = configuration.GetSection("DataBaseSettings").Get<DataBaseSettings>() 
                         ?? throw new InvalidOperationException("DataBaseSettings section is missing in configuration.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found."),
                npgsqlOptions => npgsqlOptions.MigrationsHistoryTable(
                    dbSettings.MigrationTable ?? "__EFMigrationsHistory",
                    dbSettings.DefaultSchema ?? "public"
                )
            )
        );
    }
}