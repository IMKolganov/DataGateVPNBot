using DataGateVPNBotV1.Models.Enums;
using DataGateVPNBotV1.Services;
using DataGateVPNBotV1.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace DataGateVPNBotV1.Handlers;

public partial class TelegramUpdateHandler : IUpdateHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly IOpenVpnClientService _openVpnClientService;
    private readonly ITelegramSettingsService _telegramSettingsService;
    private readonly ILogger<TelegramUpdateHandler> _logger;
    private readonly string _pathBotLog = "bot.log";

    private readonly InputPollOption[] _pollOptions =
    [
        new InputPollOption("Hello"),
        new InputPollOption("World!")
    ];

    public TelegramUpdateHandler(
        ITelegramBotClient botClient,
        IServiceProvider serviceProvider,
        IOpenVpnClientService openVpnClientService,
        ITelegramSettingsService telegramSettingsService,
        ILogger<TelegramUpdateHandler> logger)
    {
        _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _openVpnClientService = openVpnClientService ?? throw new ArgumentNullException(nameof(openVpnClientService));
        _telegramSettingsService = telegramSettingsService ?? throw new ArgumentNullException(nameof(telegramSettingsService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    #region HandleErrorAsync: Error handling for Telegram Bot API
    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
        HandleErrorSource source, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HandleError: {Exception}", exception);
        // Cooldown in case of network connection error
        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }
    #endregion


    #region  Handles incoming updates from Telegram Bot API and routes them to specific handlers.
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await (update switch
        {
            { Message: { } message } => OnMessage(message),
            { EditedMessage: { } message } => OnMessage(message),
            { CallbackQuery: { } callbackQuery } => OnCallbackQuery(callbackQuery),
            { InlineQuery: { } inlineQuery } => OnInlineQuery(inlineQuery),
            { ChosenInlineResult: { } chosenInlineResult } => OnChosenInlineResult(chosenInlineResult),
            { Poll: { } poll } => OnPoll(poll),
            { PollAnswer: { } pollAnswer } => OnPollAnswer(pollAnswer),
            // ChannelPost:
            // EditedChannelPost:
            // ShippingQuery:
            // PreCheckoutQuery:
            _ => UnknownUpdateHandlerAsync(update)
        });
    }
    #endregion
    
    #region OnMessage: Handle incoming messages
    private async Task OnMessage(Message msg)
    {
        _logger.LogInformation("Received message type: {MessageType}", msg.Type);
        if (msg.Text is not { } messageText)
            return;

        using var scope = _serviceProvider.CreateScope();
        var incomingMessageLogService = scope.ServiceProvider.GetRequiredService<IIncomingMessageLogService>();
        await incomingMessageLogService.Log(_botClient, msg);

        // Register new user if applicable
        var registrationService = scope.ServiceProvider.GetRequiredService<ITelegramRegistrationService>();
        await RegisterNewUserAsync(msg, registrationService);

        Message sentMessage = await ProcessingMessage(msg, messageText);
        _logger.LogInformation("Message sent with id: {SentMessageId}", sentMessage.Id);
    }
    #endregion

    private async Task<Message> ProcessingMessage(Message msg, string messageText)
    {
        var commandParts = messageText.Split(' ', 2);
        var command = commandParts[0].ToLower();
        // var argument = commandParts.Length > 1 ? commandParts[1] : null;

        return await (command switch
        {
            "/about_bot" => AboutBot(msg),
            "/how_to_use" => HowToUseVpn(msg),
            "/register" => RegisterForVpn(msg),
            "/get_my_files" => GetMyFiles(msg),
            "/make_new_file" => MakeNewVpnFile(msg),
            "/delete_selected_file" => DeleteSelectedFile(msg),
            "/delete_all_files" => DeleteAllFiles(msg),
            "/install_client" => InstallClient(msg),
            "/about_project" => AboutProject(msg),
            "/contacts" => Contacts(msg),
            "/change_language" => SelectLanguage(msg),
            
            "/register_commands" => RegisterCommandsAsync(msg),
            
            "/get_logs" => GetLogs(msg),
            "/get_file_log" => SendFileLog(msg),
            
            "/english" => ChangeLanguage(msg, command),
            "/русский" => ChangeLanguage(msg, command),
            "/ελληνικά" => ChangeLanguage(msg, command),

            "/photo" => SendPhoto(msg),
            "/inline_buttons" => SendInlineKeyboard(msg),
            "/keyboard" => SendReplyKeyboard(msg),
            "/remove" => RemoveKeyboard(msg),
            "/request" => RequestContactAndLocation(msg),
            "/inline_mode" => StartInlineQuery(msg),
            "/poll" => SendPoll(msg),
            "/poll_anonymous" => SendAnonymousPoll(msg),
            "/throw" => FailingHandler(),

            _ => Usage(msg)
        });
    }

    private async Task<Message> Usage(Message msg)
    {
        return await _botClient.SendMessage(msg.Chat, 
            await GetLocalizationTextAsync("BotMenu", msg.From!.Id)
            , parseMode: ParseMode.Html,
            replyMarkup: new ReplyKeyboardRemove());
    }
    
    private async Task OnCallbackQuery(CallbackQuery callbackQuery)
    {
        _logger.LogInformation("Received inline keyboard callback from: {CallbackQueryId}", callbackQuery.Id);
        await _botClient.AnswerCallbackQuery(callbackQuery.Id, "Processing your request...");

        if (callbackQuery.Data != null && callbackQuery.Data.StartsWith("/delete_file "))
        {
            var fileName = callbackQuery.Data.Substring("/delete_file ".Length);
            _logger.LogInformation("Deleting file: {FileName}", fileName);
            await DeleteFile(callbackQuery.From.Id, fileName);
        }
        else if (callbackQuery.Data != null && (callbackQuery.Data == "/English" || callbackQuery.Data == "/Русский" ||
                                                callbackQuery.Data == "/Ελληνικά"))
        {
            if (callbackQuery.Message != null) await ChangeLanguage(callbackQuery.Message, callbackQuery.Data);
            _logger.LogInformation("User selected language: {Language}", callbackQuery.Data);
        }
        else
        {
            _logger.LogWarning("Invalid callback data received: {CallbackData}", callbackQuery.Data);

            if (callbackQuery.Message != null)
                await _botClient.SendMessage(
                    chatId: callbackQuery.Message.Chat.Id,
                    text: "Invalid callback data received. Please try again."
                );
        }
    }
    
    private async Task RegisterNewUserAsync(Message msg, ITelegramRegistrationService registrationService)
    {
        await registrationService.RegisterUserAsync(
            telegramId: msg.From!.Id,
            username: msg.From.Username,
            firstName: msg.From.FirstName,
            lastName: msg.From.LastName
        );
    }

    private async Task<Message> RegisterForVpn(Message msg)
    {
        using var scope = _serviceProvider.CreateScope();
        var registrationService = scope.ServiceProvider.GetRequiredService<ITelegramRegistrationService>();
        if (msg.From != null)
            await RegisterNewUserAsync(msg, registrationService);

        return await _botClient.SendMessage(
            chatId: msg.Chat.Id,
            await GetLocalizationTextAsync("Registered", msg.From!.Id)
        );
    }


    
    private async Task<Message> SelectLanguage(Message msg, string textError = "")
    {
        var inlineKeyboard = new InlineKeyboardMarkup([
            [
                InlineKeyboardButton.WithCallbackData("English", "/English"),
                InlineKeyboardButton.WithCallbackData("Русский", "/Русский"),
                InlineKeyboardButton.WithCallbackData("Ελληνικά", "/Ελληνικά")
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
        Language? language = selectedLanguage switch
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
        await localizationService.SetUserLanguageAsync(msg.From!.Id, language.Value);
        
        await MakeNewVpnFile(msg);
        await InstallClient(msg);
        await Usage(msg);
        
        return await _botClient.SendMessage(
            chatId: msg.Chat.Id,
            text: await GetLocalizationTextAsync("SuccessChangeLanguage", msg.From!.Id),
            replyMarkup: new ReplyKeyboardRemove()
        );

    }

    private Task UnknownUpdateHandlerAsync(Update update)
    {
        _logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }
     
    private async Task<Message> RegisterCommandsAsync(Message msg)
    {
        await _botClient.SetMyCommands(_telegramSettingsService.GetTelegramMenuByLanguage(Language.English), languageCode: "en");
        await _botClient.SetMyCommands(_telegramSettingsService.GetTelegramMenuByLanguage(Language.Russian), languageCode: "ru");
        await _botClient.SetMyCommands(_telegramSettingsService.GetTelegramMenuByLanguage(Language.Greek), languageCode: "el");
        return await _botClient.SendMessage(
            chatId: msg.Chat.Id,
            text: "\u2705 All commands have been successfully registered...",
            replyMarkup: new ReplyKeyboardRemove()
        );
    }

    private async Task<string> GetLocalizationTextAsync(string key, long telegramId)
    {
        using var scope = _serviceProvider.CreateScope();
        var localizationService = scope.ServiceProvider.GetRequiredService<ILocalizationService>();
        return await localizationService.GetTextAsync(key, telegramId);
    }
}