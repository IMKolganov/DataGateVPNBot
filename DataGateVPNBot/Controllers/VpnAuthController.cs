using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;

namespace DataGateVPNBot.Controllers;

public class VpnAuthController
{
    private static readonly ConcurrentDictionary<string, TaskCompletionSource<bool>> PendingRequests = new();
    private readonly TelegramBotClient _botClient;

    public VpnAuthController()
    {
        _botClient = new TelegramBotClient("YOUR_TELEGRAM_BOT_TOKEN");
    }

    [HttpPost("auth")]
    public async Task<IActionResult> Authenticate([FromForm] string username, [FromForm] string password)
    {
        throw new NotImplementedException();
        // string requestId = Guid.NewGuid().ToString();
        // long telegramId = GetTelegramIdByUsername(username);
        //
        // var tcs = new TaskCompletionSource<bool>();
        // PendingRequests[requestId] = tcs;
        //
        // var keyboard = new InlineKeyboardMarkup(new[]
        // {
        //     new[] { InlineKeyboardButton.WithCallbackData("✅ Подтвердить", $"approve_{requestId}") },
        //     new[] { InlineKeyboardButton.WithCallbackData("❌ Отклонить", $"deny_{requestId}") }
        // });
        //
        // await _botClient.SendTextMessageAsync(telegramId, 
        //     $"Запрос на подключение к VPN\nПользователь: {username}\nПодтвердить?",
        //     replyMarkup: keyboard);
        //
        // // Ждём ответа пользователя (таймаут 30 сек)
        // bool isApproved = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(30))) == tcs.Task && tcs.Task.Result;
        //
        // PendingRequests.TryRemove(requestId, out _);
        // return isApproved ? Ok("OK") : Unauthorized("DENIED");
    }

    private long GetTelegramIdByUsername(string username)
    {
        throw new NotImplementedException();

        // Тут можешь использовать свою базу, где логины OpenVPN связаны с Telegram ID
        return 123456789; // Заглушка (замени на реальную логику)
    }

    [HttpPost("callback")]
    public async Task<IActionResult> HandleCallback([FromBody] Telegram.Bot.Types.CallbackQuery callbackQuery)
    {
        throw new NotImplementedException();
        // if (callbackQuery.Data.StartsWith("approve_") || callbackQuery.Data.StartsWith("deny_"))
        // {
        //     string requestId = callbackQuery.Data.Split('_')[1];
        //
        //     if (PendingRequests.TryGetValue(requestId, out var tcs))
        //     {
        //         bool isApproved = callbackQuery.Data.StartsWith("approve_");
        //         tcs.TrySetResult(isApproved);
        //
        //         await _botClient.EditMessageTextAsync(callbackQuery.Message.Chat.Id, 
        //             callbackQuery.Message.MessageId,
        //             isApproved ? "✅ Доступ разрешён!" : "❌ Доступ запрещён!");
        //
        //         return Ok();
        //     }
        // }
        // return BadRequest();
    }
}