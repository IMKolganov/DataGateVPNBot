﻿using DataGateVPNBot.DataBase.Contexts;
using DataGateVPNBot.Models.Helpers;
using Microsoft.EntityFrameworkCore;

namespace DataGateVPNBot.Configurations;

public static class DataBaseConfigurations
{
    public static void DataBaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        var dbSettings = configuration.GetSection("DataBaseSettings").Get<DataBaseSettings>() 
                         ?? throw new InvalidOperationException("DataBaseSettings section is missing in configuration.");

        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            var config = serviceProvider.GetRequiredService<IConfiguration>();
            options.UseNpgsql(
                config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found."),
                npgsqlOptions => npgsqlOptions.MigrationsHistoryTable(
                    dbSettings.MigrationTable ?? "__EFMigrationsHistory",
                    dbSettings.DefaultSchema ?? "public"
                )
            );
        });
    }
}