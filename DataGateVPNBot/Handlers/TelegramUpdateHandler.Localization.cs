using DataGateVPNBot.Models.Enums;
using DataGateVPNBot.Services.DataServices.Interfaces;
using DataGateVPNBot.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace DataGateVPNBot.Handlers;

public partial class TelegramUpdateHandler
{
    private async Task<Message> SelectLanguage(Message msg, string textError = "")
    {
        var inlineKeyboard = new InlineKeyboardMarkup([
            [
                InlineKeyboardButton.WithCallbackData("English", "/english".ToLower()),
                InlineKeyboardButton.WithCallbackData("Русский", "/русский".ToLower()),
                InlineKeyboardButton.WithCallbackData("Ελληνικά", "/ελληνικά".ToLower())
            ]
        ]);

        return await _botClient.SendMessage(
            chatId: msg.Chat.Id,
            text: textError + "🔹 You can click on your preferred language to proceed.\n" +
                  "🔹 Выберите ваш язык, нажав на соответствующую кнопку.\n" +
                  "🔹 Επιλέξτε τη γλώσσα σας πατώντας το αντίστοιχο κουμπί.",
            replyMarkup: inlineKeyboard
        );
    }

    private async Task<Message> ChangeLanguage(Message msg, string selectedLanguage)
    {
        Language? language = selectedLanguage.ToLower() switch
        {
            "/english" => Language.English,
            "/русский" => Language.Russian,
            "/ελληνικά" => Language.Greek,
            _ => null
        };

        if (language == null)
        {
            return await SelectLanguage(msg, "❌ Invalid language selection. Please try again.");
        }

        using var scope = _serviceProvider.CreateScope();
        var localizationService = scope.ServiceProvider.GetRequiredService<ILocalizationService>();
        await localizationService.SetUserLanguageAsync(msg.Chat.Id, language.Value);
        
        Message messageResponse = await _botClient.SendMessage(
            chatId: msg.Chat.Id,
            text: await GetLocalizationTextAsync("SuccessChangeLanguage", msg.Chat.Id),
            replyMarkup: new ReplyKeyboardRemove()
        );
        
        await MakeNewVpnFile(msg);
        await InstallClient(msg);
        await Usage(msg);

        return messageResponse;
    }
    
    private async Task<string> GetLocalizationTextAsync(string key, long telegramId)
    {
        using var scope = _serviceProvider.CreateScope();
        var localizationService = scope.ServiceProvider.GetRequiredService<ILocalizationService>();
        return await localizationService.GetTextAsync(key, telegramId);
    }
}