using DataGateVPNBotV1.Configurations;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureTelegram(builder.Configuration);
builder.Services.ConfigureServices();
builder.Services.DataBaseServices(builder.Configuration);

builder.Host.ConfigureSerilog(builder.Configuration);
builder.ConfigureWebHost();

builder.ConfigureWebHost();

var app = builder.Build();

app.ConfigureMiddleware();
app.ConfigurePipeline();

app.Run();