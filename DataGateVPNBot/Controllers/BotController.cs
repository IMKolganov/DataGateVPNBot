using DataGateVPNBot.Handlers;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DataGateVPNBot.Controllers;

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