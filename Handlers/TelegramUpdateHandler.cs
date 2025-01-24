using DataGateVPNBotV1.Models.Configurations;
using DataGateVPNBotV1.Models.Enums;
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
    private readonly string _pathBotLog;
    
    public TelegramUpdateHandler(
        ITelegramBotClient botClient,
        IServiceProvider serviceProvider,
        IOpenVpnClientService openVpnClientService,
        ITelegramSettingsService telegramSettingsService,
        ILogger<TelegramUpdateHandler> logger,
        IConfiguration configuration)
    {
        _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _openVpnClientService = openVpnClientService ?? throw new ArgumentNullException(nameof(openVpnClientService));
        _telegramSettingsService = telegramSettingsService ?? throw new ArgumentNullException(nameof(telegramSettingsService));
        _pathBotLog = configuration.GetSection("BotConfiguration").Get<BotConfiguration>()?.LogFile ?? throw new InvalidOperationException();
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
        await LogIncomingMessage(msg);

        Message sentMessage = await ProcessingMessage(msg, messageText);
        _logger.LogInformation("Message sent with id: {SentMessageId}", sentMessage.Id);
    }
    #endregion

    private async Task<Message> ProcessingMessage(Message msg, string messageText)
    {
        var commandParts = messageText.Split(' ', 2);
        var command = commandParts[0].ToLower();
        // var argument = commandParts.Length > 1 ? commandParts[1] : null;
        if (!await IsExistLocalizationSettings(msg.From!.Id))
        {
            _logger.LogInformation("Localization settings not found for user with TelegramId: {TelegramId}. Calling SelectLanguage.", msg.From.Id);
            await SelectLanguage(msg);
        }
        else
        {
            _logger.LogInformation("Localization settings found for user with TelegramId: {TelegramId}.", msg.From.Id);
        }
        await RegisterNewUserAsync(msg);//for something wrong when "/start" don't work. This line usually is not a necessary 

        return await (command switch
        {
            "/start" => Start(msg),
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

    private async Task<Message> Start(Message msg)
    {
        // Register new user if applicable
        await RegisterNewUserAsync(msg);

        return await SelectLanguage(msg);
    }
    
    private async Task OnCallbackQuery(CallbackQuery callbackQuery)
    {
        _logger.LogInformation("Received inline keyboard callback from: {CallbackQueryId}", callbackQuery.Id);
        await _botClient.AnswerCallbackQuery(callbackQuery.Id, "Processing your request...");

        if (callbackQuery.Data != null && callbackQuery.Data.ToLower().StartsWith("/delete_file "))
        {
            var fileName = callbackQuery.Data.Substring("/delete_file ".Length);
            _logger.LogInformation("Deleting file: {FileName}", fileName);
            await DeleteFile(callbackQuery.From.Id, fileName);
        }
        else if (callbackQuery.Data != null && (callbackQuery.Data.ToLower() == "/english" || 
                                                callbackQuery.Data.ToLower() == "/русский" ||
                                                callbackQuery.Data.ToLower() == "/ελληνικά"))
        {
            if (callbackQuery.Message != null) await ChangeLanguage(callbackQuery.Message, 
                callbackQuery.Data.ToLower());
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
    
    private async Task<Message> RegisterForVpn(Message msg)
    {
        if (msg.From != null)
            await RegisterNewUserAsync(msg);

        return await _botClient.SendMessage(
            chatId: msg.Chat.Id,
            await GetLocalizationTextAsync("Registered", msg.From!.Id)
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
    
    private async Task RegisterNewUserAsync(Message msg)
    {
        using var scope = _serviceProvider.CreateScope();
        var registrationService = scope.ServiceProvider.GetRequiredService<ITelegramUsersService>();
        await registrationService.RegisterUserAsync(
            telegramId: msg.From!.Id,
            username: msg.From.Username,
            firstName: msg.From.FirstName,
            lastName: msg.From.LastName
        );
    }

    private async Task LogIncomingMessage(Message msg)
    {
        using var scope = _serviceProvider.CreateScope();
        var incomingMessageLogService = scope.ServiceProvider.GetRequiredService<IIncomingMessageLogService>();
        await incomingMessageLogService.Log(_botClient, msg);
    }
    
    private async Task<bool> IsExistLocalizationSettings(long telegramId)
    {
        _logger.LogInformation("Checking localization settings for TelegramId: {TelegramId}.", telegramId);
        using var scope = _serviceProvider.CreateScope();
        var incomingMessageLogService = scope.ServiceProvider.GetRequiredService<ILocalizationService>();

        var result = await incomingMessageLogService.IsExistUserLanguageAsync(telegramId);
        _logger.LogInformation("Result of IsExistUserLanguageAsync for TelegramId {TelegramId}: {Result}", telegramId, result);

        return result;
    }
}