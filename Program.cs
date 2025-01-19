using System.Security.Cryptography.X509Certificates;
using DataGateVPNBotV1.Contexts;
using DataGateVPNBotV1.Models;
using DataGateVPNBotV1.Services;
using DataGateVPNBotV1.Services.Interfaces;
using Telegram.Bot;
using Microsoft.EntityFrameworkCore;
    
var builder = WebApplication.CreateBuilder(args);

var botConfigSection = builder.Configuration.GetSection("BotConfiguration");
builder.Services.Configure<BotConfiguration>(botConfigSection);
builder.Services.AddHttpClient("tgwebhook").AddTypedClient<ITelegramBotClient>(
    httpClient => new TelegramBotClient(botConfigSection.Get<BotConfiguration>()!.BotToken, httpClient));


builder.Services.AddScoped<IIssuedOvpnFileService, IssuedOvpnFileService>();
builder.Services.AddScoped<IIncomingMessageLogService, IncomingMessageLogService>();
builder.Services.AddScoped<ITelegramRegistrationService, TelegramRegistrationService>();
builder.Services.AddScoped<ILocalizationService, LocalizationService>();
builder.Services.AddSingleton<UpdateHandler>();
builder.Services.AddSingleton<IOpenVpnClientService, OpenVpnClientService>();

builder.Services.ConfigureTelegramBotMvc();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.UseUrls("http://localhost:8443", "https://localhost:8443");

var certificate = X509Certificate2.CreateFromPemFile("datagatetgbot.pem", "datagatetgbot.key");

builder.WebHost.UseKestrel(options =>
{
    options.ListenAnyIP(8443, listenOptions =>
    {
        listenOptions.UseHttps(certificate);
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? throw new InvalidOperationException(),
        npgsqlOptions => npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "xgb_rackotpg")
    )
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();