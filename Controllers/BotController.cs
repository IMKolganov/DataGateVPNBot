using DataGateVPNBotV1.Models.Configurations;
using DataGateVPNBotV1.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DataGateVPNBotV1.Controllers;

[ApiController]
[Route("[controller]")]
public class BotController : ControllerBase
{
    public BotController()
    {
    }

    [HttpGet(Name = "healthcheck")]
    public IActionResult Healthcheck()
    {
        return Ok(200);
    }
    
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update update, [FromServices] ITelegramBotClient bot, 
        [FromServices] TelegramUpdateHandler handleTelegramUpdateService, CancellationToken ct)
    {
        try
        {
            await handleTelegramUpdateService.HandleUpdateAsync(bot, update, ct);
        }
        catch (Exception exception)
        {
            await handleTelegramUpdateService.HandleErrorAsync(bot, exception, Telegram.Bot.Polling.HandleErrorSource.HandleUpdateError, ct);
        }
        return Ok();
    }
}