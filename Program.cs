using DataGateVPNBotV1.Configurations;
using DataGateVPNBotV1.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureTelegram(builder.Configuration);
builder.Services.ConfigureServices();
builder.Services.DataBaseServices(builder.Configuration);


builder.ConfigureWebHost();

var app = builder.Build();

app.ConfigureMiddleware();
app.ConfigurePipeline();

app.Run();