using DataGateVPNBotV1.Contexts;
using Microsoft.EntityFrameworkCore;

namespace DataGateVPNBotV1.Configurations;

public static class DataBaseConfigurations
{
    public static void DataBaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found."),
                npgsqlOptions => npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "xgb_rackotpg")//todo: move to config
            )
        );
    }
}