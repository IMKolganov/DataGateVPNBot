using DataGateVPNBotV1;
using DataGateVPNBotV1.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureServices(builder.Configuration);
builder.Services.DataBaseServices(builder.Configuration);

builder.ConfigureWebHost();

var app = builder.Build();

app.ConfigurePipeline();

app.Run();