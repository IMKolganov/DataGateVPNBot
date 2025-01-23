using DataGateVPNBotV1.Models;
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
    private readonly IOptions<BotConfiguration> _config;
    
    public BotController(IOptions<BotConfiguration> config)
    {
        _config = config;
    }

    [HttpGet(Name = "healthcheck")]
    public IActionResult Healthcheck()
    {
        return Ok(200);
    }
    
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update update, [FromServices] ITelegramBotClient bot, 
        [FromServices] UpdateHandler handleUpdateService, CancellationToken ct)
    {
        // if (Request.Headers["X-Telegram-Bot-Api-Secret-Token"] != _config.Value.SecretToken)
        //     return Forbid();
        try
        {
            await handleUpdateService.HandleUpdateAsync(bot, update, ct);
        }
        catch (Exception exception)
        {
            await handleUpdateService.HandleErrorAsync(bot, exception, Telegram.Bot.Polling.HandleErrorSource.HandleUpdateError, ct);
        }
        return Ok();
    }
}