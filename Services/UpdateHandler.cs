using DataGateVPNBotV1.Models.Enums;
using DataGateVPNBotV1.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;


namespace DataGateVPNBotV1.Services;

public class UpdateHandler : IUpdateHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly IOpenVpnClientService _openVpnClientService;
    private readonly ILogger<UpdateHandler> _logger;
    private readonly string _pathBotLog = "bot.log";

    private readonly InputPollOption[] _pollOptions = new[]
    {
        new InputPollOption("Hello"),
        new InputPollOption("World!")
    };

    public UpdateHandler(
        ITelegramBotClient botClient,
        IServiceProvider serviceProvider,
        IOpenVpnClientService openVpnClientService,
        ILogger<UpdateHandler> logger)
    {
        _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _openVpnClientService = openVpnClientService ?? throw new ArgumentNullException(nameof(openVpnClientService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
        HandleErrorSource source, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HandleError: {Exception}", exception);
        // Cooldown in case of network connection error
        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }

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

    private async Task OnMessage(Message msg)
    {
        _logger.LogInformation("Receive message type: {MessageType}", msg.Type);
        if (msg.Text is not { } messageText)
            return;

        using var scope = _serviceProvider.CreateScope(); //todo: fix this shit

        var incomingMessageLogService = scope.ServiceProvider.GetRequiredService<IIncomingMessageLogService>();
        await incomingMessageLogService.Log(_botClient, msg);

        var registrationService = scope.ServiceProvider.GetRequiredService<ITelegramRegistrationService>();
        await RegisterNewUserAsync(msg, registrationService);

        var localizationService = scope.ServiceProvider.GetRequiredService<ILocalizationService>();

        if (messageText.Equals("/English", StringComparison.OrdinalIgnoreCase) ||
            messageText.Equals("/Русский", StringComparison.OrdinalIgnoreCase) ||
            messageText.Equals("/Ελληνικά", StringComparison.OrdinalIgnoreCase))
        {
            await ChangeLanguage(msg);
            return;
        }

        if (msg.From != null)
        {
            var userLanguage = await localizationService.GetUserLanguageOrNullAsync(msg.From.Id);
            if (userLanguage == null)
            {
                await SelectLanguage(msg);
                return;
            }
        }

        Message sentMessage = await Menu(msg, messageText);
        _logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.Id);
    }

    async Task<Message> Menu(Message msg, string messageText)
    {
        var commandParts = messageText.Split(' ', 2);
        var command = commandParts[0];
        var argument = commandParts.Length > 1 ? commandParts[1] : null;
        
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

    async Task<Message> Usage(Message msg)
    {
        using var scope = _serviceProvider.CreateScope();
        var localizationService = scope.ServiceProvider.GetRequiredService<ILocalizationService>();
        string usage = await localizationService.GetTextAsync("BotMenu", msg.From!.Id);
        return await _botClient.SendMessage(msg.Chat, usage, parseMode: ParseMode.Html,
            replyMarkup: new ReplyKeyboardRemove());
    }

    async Task<Message> AboutBot(Message msg)
    {
        using var scope = _serviceProvider.CreateScope();
        var localizationService = scope.ServiceProvider.GetRequiredService<ILocalizationService>();
        string roustabout = await localizationService.GetTextAsync("AboutBot", msg.From!.Id);
        return await _botClient.SendMessage(
            msg.Chat,
            roustabout
        );
    }

    async Task<Message> RegisterForVpn(Message msg)
    {
        using var scope = _serviceProvider.CreateScope();
        var registrationService = scope.ServiceProvider.GetRequiredService<ITelegramRegistrationService>();
        var localizationService = scope.ServiceProvider.GetRequiredService<ILocalizationService>();
        string registered = await localizationService.GetTextAsync("Registered", msg.From!.Id);
        if (msg.From != null)
            await RegisterNewUserAsync(msg, registrationService);

        return await _botClient.SendMessage(
            chatId: msg.Chat.Id,
            text: registered
        );
    }

    async Task RegisterNewUserAsync(Message msg, ITelegramRegistrationService registrationService)
    {
        await registrationService.RegisterUserAsync(
            telegramId: msg.From!.Id,
            username: msg.From.Username,
            firstName: msg.From.FirstName,
            lastName: msg.From.LastName
        );
    }

    async Task<Message> HowToUseVpn(Message msg)
    {
        using var scope = _serviceProvider.CreateScope();
        var localizationService = scope.ServiceProvider.GetRequiredService<ILocalizationService>();
        var response = await localizationService.GetTextAsync("HowToUseVPN", msg.From!.Id);
        return await _botClient.SendMessage(
            msg.Chat,
            response);
    }

    async Task<Message> GetMyFiles(Message msg)
    {
        _logger.LogInformation("GetMyFiles started for user: {TelegramId}", msg.From?.Id);

        try
        {
            _logger.LogInformation("Fetching client configurations...");
            var clientConfigFiles = await _openVpnClientService.GetAllClientConfigurations(msg.From!.Id);
            _logger.LogInformation("Fetched {Count} configuration files.", clientConfigFiles.FileInfo.Count);

            if (clientConfigFiles.FileInfo.Count <= 0)
            {
                using var scope = _serviceProvider.CreateScope();
                var localizationService = scope.ServiceProvider.GetRequiredService<ILocalizationService>();
                string filesNotFoundErrorText = await localizationService.GetTextAsync("FilesNotFoundError", msg.From!.Id);
                return await _botClient.SendMessage(
                    chatId: msg.Chat.Id,
                    text: filesNotFoundErrorText,
                    replyMarkup: new ReplyKeyboardRemove()
                );
            }

            if (clientConfigFiles.FileInfo.Count >= 2)
            {
                _logger.LogInformation("Multiple configuration files detected. Preparing media group...");
                var mediaGroup = new List<IAlbumInputMedia>();
                var openStreams = new List<FileStream>();

                try
                {
                    foreach (var fileInfo in clientConfigFiles.FileInfo)
                    {
                        _logger.LogInformation("Processing file: {FileName} at {FilePath}", fileInfo.Name, fileInfo.FullName);

                        var fileStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
                        openStreams.Add(fileStream);

                        var inputFile = new InputFileStream(fileStream, fileInfo.Name);
                        var media = new InputMediaDocument(inputFile)
                        {
                            Caption = fileInfo.Name
                        };
                        mediaGroup.Add(media);
                    }

                    _logger.LogInformation("Sending media group...");
                    var m = await _botClient.SendMediaGroup(
                        chatId: msg.Chat.Id,
                        media: mediaGroup
                    );
                    _logger.LogInformation("Media group sent successfully.");

                    return m.FirstOrDefault() ?? throw new InvalidOperationException("No messages returned after sending media group.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while sending media group.");
                    throw;
                }
                finally
                {
                    foreach (var stream in openStreams)
                    {
                        stream.Close();
                        _logger.LogInformation("Closed stream for file: {StreamPath}", stream.Name);
                    }
                }
            }
            else
            {
                await _botClient.SendChatAction(msg.Chat.Id, ChatAction.UploadDocument);
                _logger.LogInformation("Single configuration file detected.");
                var clientConfigFile = clientConfigFiles.FileInfo.FirstOrDefault()
                                       ?? throw new InvalidOperationException("No configuration file found.");

                _logger.LogInformation("Reading file: {FileName} from {FilePath}", clientConfigFile.Name,
                    clientConfigFile.FullName);
                await using var fileStream = new FileStream(clientConfigFile.FullName,
                    FileMode.Open, FileAccess.Read, FileShare.Read);

                _logger.LogInformation("Sending single document...");
                var sentMessage = await _botClient.SendDocument(
                    chatId: msg.Chat.Id,
                    document: InputFile.FromStream(fileStream, clientConfigFile.Name),
                    caption: clientConfigFiles.Message
                );
                _logger.LogInformation("Document sent successfully.");

                return sentMessage;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in GetMyFiles for user: {TelegramId}", msg.From?.Id);
            throw;
        }
    }

    async Task<Message> MakeNewVpnFile(Message msg)
    {
        // Generate the client configuration file
        var clientConfigFile = await _openVpnClientService.CreateClientConfiguration(msg.From!.Id);
        if (clientConfigFile.FileInfo != null)
        {
            _logger.LogInformation("Client configuration created successfully in UpdateHandler.");
            await _botClient.SendChatAction(msg.Chat.Id, ChatAction.UploadDocument);
            // Send the .ovpn file to the user
            await using var fileStream = new FileStream(clientConfigFile.FileInfo.FullName, FileMode.Open, FileAccess.Read,
                FileShare.Read);
            return await _botClient.SendDocument(
                chatId: msg.Chat.Id,
                document: InputFile.FromStream(fileStream, clientConfigFile.FileInfo.Name),
                caption: clientConfigFile.Message
            );
        }
        else
        {
            return await _botClient.SendMessage(
                chatId: msg.Chat.Id,
                text: clientConfigFile.Message,
                replyMarkup: new ReplyKeyboardRemove()
            );
        }
    }
    
    async Task<Message> DeleteAllFiles(Message msg)
    {
        using var scope = _serviceProvider.CreateScope();
        var localizationService = scope.ServiceProvider.GetRequiredService<ILocalizationService>();
        string successfullyDeletedAllFile = await localizationService.GetTextAsync("SuccessfullyDeletedAllFile", msg.From.Id);
        
        await _openVpnClientService.DeleteAllClientConfigurations(msg.From!.Id);
        return await _botClient.SendMessage(
            chatId: msg.Chat.Id,
            text: successfullyDeletedAllFile,
            replyMarkup: new ReplyKeyboardRemove()
        );
    }

    async Task<Message> DeleteSelectedFile(Message msg)
    {
        using var scope = _serviceProvider.CreateScope();
        var localizationService = scope.ServiceProvider.GetRequiredService<ILocalizationService>();
        string chooseFileForDeleteText = await localizationService.GetTextAsync("ChooseFileForDelete", msg.From!.Id);
        
        var clientConfigFiles = await _openVpnClientService.GetAllClientConfigurations(msg.From!.Id);
        var rows = new List<InlineKeyboardButton[]>();

        var currentRow = new List<InlineKeyboardButton>();
        foreach (var fileInfo in clientConfigFiles.FileInfo)
        {
            currentRow.Add(InlineKeyboardButton.WithCallbackData(fileInfo.Name, $"/delete_file {fileInfo.Name}"));

            if (currentRow.Count == 2)
            {
                rows.Add(currentRow.ToArray());
                currentRow.Clear();
            }
        }
        
        if (currentRow.Count > 0)
        {
            rows.Add(currentRow.ToArray());
        }

        var inlineMarkup = new InlineKeyboardMarkup(rows);
        return await _botClient.SendMessage(
            msg.Chat,
            chooseFileForDeleteText,
            replyMarkup: inlineMarkup
        );
    }

    async Task<Message> DeleteFile(long telegramId, string fileName)
    {
        using var scope = _serviceProvider.CreateScope();
        var localizationService = scope.ServiceProvider.GetRequiredService<ILocalizationService>();
        string successfullyDeletedFileText = await localizationService.GetTextAsync("SuccessfullyDeletedFile", telegramId);
        
        await _openVpnClientService.DeleteClientConfiguration(telegramId, fileName);
        return await _botClient.SendMessage(
            chatId: telegramId,
            text: successfullyDeletedFileText,
            replyMarkup: new ReplyKeyboardRemove()
        );
    }

    async Task<Message> InstallClient(Message msg)
    {
        using var scope = _serviceProvider.CreateScope();
        var localizationService = scope.ServiceProvider.GetRequiredService<ILocalizationService>();
        string choosePlatformText = await localizationService.GetTextAsync("ChoosePlatform", msg.From!.Id);
        string aboutOpenVpnText = await localizationService.GetTextAsync("AboutOpenVPN", msg.From!.Id);
        var inlineMarkup = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithUrl("🖥 Windows", "https://openvpn.net/client-connect-vpn-for-windows/"),
                InlineKeyboardButton.WithUrl("📱 Android", "https://play.google.com/store/apps/details?id=net.openvpn.openvpn"),
                InlineKeyboardButton.WithUrl("🍎 iPhone", "https://apps.apple.com/app/openvpn-connect/id590379981")
            },
            new[]
            {
                InlineKeyboardButton.WithUrl(aboutOpenVpnText, "https://openvpn.net/faq/what-is-openvpn/")
            }
        });

        return await _botClient.SendMessage(
            msg.Chat,
            choosePlatformText,
            replyMarkup: inlineMarkup
        );
    }
    
    async Task<Message> AboutProject(Message msg)
    {
        using var scope = _serviceProvider.CreateScope();
        var localizationService = scope.ServiceProvider.GetRequiredService<ILocalizationService>();
        string aboutProjectText = await localizationService.GetTextAsync("AboutProject", msg.From!.Id);
        string whatIsRaspberryPitext = await localizationService.GetTextAsync("WhatIsRaspberryPi", msg.From!.Id);
        var inlineMarkup = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithUrl(whatIsRaspberryPitext, "https://www.raspberrypi.org/about/")
            }
        });
        
        return await _botClient.SendMessage(
            msg.Chat, 
            aboutProjectText,
            replyMarkup: inlineMarkup
        );
    }
    
    async Task<Message> Contacts(Message msg)
    {
        using var scope = _serviceProvider.CreateScope();
        var localizationService = scope.ServiceProvider.GetRequiredService<ILocalizationService>();
        string developercontactstext = await localizationService.GetTextAsync("DeveloperContacts", msg.From!.Id);
        var inlineMarkup = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithUrl("Telegram", "https://t.me/KolganovIvan"),
                InlineKeyboardButton.WithUrl("GitHub", "https://github.com/IMKolganov")
            }
        });
        
        return await _botClient.SendMessage(
            msg.Chat,
            developercontactstext,
            replyMarkup: inlineMarkup
        );
    }

    async Task<Message> SelectLanguage(Message msg)
    {
        var replyMarkup = new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { "/English", "/Русский", "/Ελληνικά" }
        })
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = true
        };

        return await _botClient.SendMessage(
            chatId: msg.Chat.Id,
            text: "🔹 You can click on your preferred language to proceed.\n" +
                  "🔹 Выберите ваш язык, нажав на соответствующую кнопку.\n" +
                  "🔹 Επιλέξτε τη γλώσσα σας πατώντας το αντίστοιχο κουμπί.",
            replyMarkup: replyMarkup
        );

    }

    async Task ChangeLanguage(Message msg)
    {
        var selectedLanguage = msg.Text;
        Language? language = selectedLanguage switch
        {
            "/English" => Language.English,
            "/Русский" => Language.Russian,
            "/Ελληνικά" => Language.Greek,
            _ => null
        };

        if (language == null)
        {
            await _botClient.SendMessage(
                chatId: msg.Chat.Id,
                text: "❌ Invalid language selection. Please try again.",
                replyMarkup: new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton[] { "/English", "/Русский", "/Ελληνικά" }
                })
                {
                    ResizeKeyboard = true,
                    OneTimeKeyboard = true
                }
            );
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var localizationService = scope.ServiceProvider.GetRequiredService<ILocalizationService>();
        await localizationService.SetUserLanguageAsync(msg.From!.Id, language.Value);
        string confirmationMessage = await localizationService.GetTextAsync("SuccessChangeLanguage", msg.From.Id);

        await _botClient.SendMessage(
            chatId: msg.Chat.Id,
            text: confirmationMessage,
            replyMarkup: new ReplyKeyboardRemove()
        );
        await MakeNewVpnFile(msg);
        await InstallClient(msg);
        await Usage(msg);
    }

    async Task<Message> GetLogs(Message msg, int linesToRead = 100)
    {
        if (!File.Exists(_pathBotLog))
            throw new FileNotFoundException($"Log file not found: {_pathBotLog}");

        var lines = new LinkedList<string>();

        using (var fileStream = new FileStream(_pathBotLog, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            fileStream.Seek(0, SeekOrigin.End);

            long position = fileStream.Position;
            int bufferSize = 1024;
            var buffer = new char[bufferSize];
            int bytesRead;
            var currentLine = new LinkedList<char>();

            using (var streamReader = new StreamReader(fileStream))
            {
                while (position > 0 && lines.Count < linesToRead)
                {
                    position -= bufferSize;
                    if (position < 0)
                    {
                        bufferSize += (int)position;
                        position = 0;
                    }

                    fileStream.Seek(position, SeekOrigin.Begin);
                    bytesRead = await streamReader.ReadAsync(buffer, 0, bufferSize);

                    for (int i = bytesRead - 1; i >= 0; i--)
                    {
                        if (buffer[i] == '\n')
                        {
                            lines.AddFirst(new string(currentLine.ToArray()));
                            currentLine.Clear();

                            if (lines.Count >= linesToRead)
                                break;
                        }
                        else
                        {
                            currentLine.AddFirst(buffer[i]);
                        }
                    }
                }

                if (currentLine.Count > 0)
                    lines.AddFirst(new string(currentLine.ToArray()));
            }
        }

        var logText = string.Join(Environment.NewLine, lines);

        if (logText.Length > 4096)
        {
            logText = logText.Substring(logText.Length - 4093) + "...";
        }

        return await _botClient.SendMessage(
            chatId: msg.Chat.Id,
            text: logText,
            replyMarkup: new ReplyKeyboardRemove()
        );
    }

    async Task<Message> SendFileLog(Message msg)
    {
        if (!File.Exists(_pathBotLog))
            throw new FileNotFoundException($"Log file not found: {_pathBotLog}");

        await _botClient.SendChatAction(msg.Chat.Id, ChatAction.UploadDocument);

        using var fileStream = new FileStream(_pathBotLog, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        try
        {
            return await _botClient.SendDocument(
                chatId: msg.Chat.Id,
                document: InputFile.FromStream(fileStream, Path.GetFileName(_pathBotLog)),
                caption: "Read https://github.com/IMKolganov/DataGateVPNBot"
            );
        }
        catch (Telegram.Bot.Exceptions.RequestException ex)
        {
            Console.WriteLine($"Telegram API Error: {ex.Message}");
            throw;
        }
    }
    
    async Task<Message> SendPhoto(Message msg)
    {
        await _botClient.SendChatAction(msg.Chat, ChatAction.UploadPhoto);
        await Task.Delay(2000); // simulate a long task
        await using var fileStream = new FileStream("Files/bot.gif", FileMode.Open, FileAccess.Read);
        return await _botClient.SendAnimation(msg.Chat, fileStream, caption: "Read https://github.com/IMKolganov/DataGateVPNBot");
    }

    // Send inline keyboard. You can process responses in OnCallbackQuery handler
    async Task<Message> SendInlineKeyboard(Message msg)
    {
        var inlineMarkup = new InlineKeyboardMarkup()
            .AddNewRow("1.1", "1.2", "1.3")
            .AddNewRow()
                .AddButton("WithCallbackData", "CallbackData")
                .AddButton(InlineKeyboardButton.WithUrl("WithUrl", "https://github.com/TelegramBots/Telegram.Bot"));
        return await _botClient.SendMessage(msg.Chat, "Inline buttons:", replyMarkup: inlineMarkup);
    }

    async Task<Message> SendReplyKeyboard(Message msg)
    {
        var replyMarkup = new ReplyKeyboardMarkup(true)
            .AddNewRow("1.1", "1.2", "1.3")
            .AddNewRow().AddButton("2.1").AddButton("2.2");
        return await _botClient.SendMessage(msg.Chat, "Keyboard buttons:", replyMarkup: replyMarkup);
    }

    async Task<Message> RemoveKeyboard(Message msg)
    {
        return await _botClient.SendMessage(msg.Chat, "Removing keyboard", replyMarkup: new ReplyKeyboardRemove());
    }

    async Task<Message> RequestContactAndLocation(Message msg)
    {
        var replyMarkup = new ReplyKeyboardMarkup(true)
            .AddButton(KeyboardButton.WithRequestLocation("Location"))
            .AddButton(KeyboardButton.WithRequestContact("Contact"));
        return await _botClient.SendMessage(msg.Chat, "Who or Where are you?", replyMarkup: replyMarkup);
    }

    async Task<Message> StartInlineQuery(Message msg)
    {
        var button = InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Inline Mode");
        return await _botClient.SendMessage(msg.Chat, "Press the button to start Inline Query\n\n" +
                                                      "(Make sure you enabled Inline Mode in @BotFather)", replyMarkup: new InlineKeyboardMarkup(button));
    }

    async Task<Message> SendPoll(Message msg)
    {
        return await _botClient.SendPoll(msg.Chat, "Question", _pollOptions, isAnonymous: false);
    }

    async Task<Message> SendAnonymousPoll(Message msg)
    {
        return await _botClient.SendPoll(chatId: msg.Chat, "Question", _pollOptions);
    }

    static Task<Message> FailingHandler()
    {
        throw new NotImplementedException("FailingHandler");
    }

    // Process Inline Keyboard callback data
    private async Task OnCallbackQuery(CallbackQuery callbackQuery)
    {
        _logger.LogInformation("Received inline keyboard callback from: {CallbackQueryId}", callbackQuery.Id);
        
        await _botClient.AnswerCallbackQuery(callbackQuery.Id, "Processing your request...");

        if (callbackQuery.Data != null && callbackQuery.Data.StartsWith("/delete_file "))
        {
            var fileName = callbackQuery.Data.Substring("/delete_file ".Length);
            
            _logger.LogInformation("Deleting file: {FileName}", fileName);

            await DeleteFile(callbackQuery.From.Id, fileName);

            // await _botClient.SendMessage(callbackQuery.Message.Chat, result);
        }
        // else
        // {
        //     await _botClient.SendMessage(callbackQuery.Message!.Chat, "Invalid callback data received.");
        // }
    }

    #region Inline Mode

    private async Task OnInlineQuery(InlineQuery inlineQuery)
    {
        _logger.LogInformation("Received inline query from: {InlineQueryFromId}", inlineQuery.From.Id);

        // throw new NotImplementedException();// displayed result
        InlineQueryResult[] results = new InlineQueryResult[] 
        {
            new InlineQueryResultArticle("1", "Telegram.Bot", new InputTextMessageContent("hello")),
            new InlineQueryResultArticle("2", "is the best", new InputTextMessageContent("world"))
        };
        await _botClient.AnswerInlineQuery(inlineQuery.Id, results, cacheTime: 0, isPersonal: true);
    }

    private async Task OnChosenInlineResult(ChosenInlineResult chosenInlineResult)
    {
        _logger.LogInformation("Received inline result: {ChosenInlineResultId}", chosenInlineResult.ResultId);
        await _botClient.SendMessage(chosenInlineResult.From.Id, $"You chose result with Id: {chosenInlineResult.ResultId}");
    }

    #endregion

    private Task OnPoll(Poll poll)
    {
        _logger.LogInformation("Received Poll info: {Question}", poll.Question);
        return Task.CompletedTask;
    }

    private async Task OnPollAnswer(PollAnswer pollAnswer)
    {
        // throw new NotImplementedException("OnPollAnswer");
        var answer = pollAnswer.OptionIds.FirstOrDefault();
        var selectedOption = _pollOptions[answer];
        if (pollAnswer.User != null)
            await _botClient.SendMessage(pollAnswer.User.Id, $"You've chosen: {selectedOption.Text} in poll");
    }

    private Task UnknownUpdateHandlerAsync(Update update)
    {
        _logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }
    
    private async Task<Message> RegisterCommandsAsync(Message msg)
    {
        var commandsEn = new[]
        {
            // new BotCommand { Command = "register", Description = "Register to use the VPN" },
            new BotCommand { Command = "get_my_files", Description = "Get your files for connecting to the VPN" },
            new BotCommand { Command = "make_new_file", Description = "Create a new file for connecting to the VPN" },
            new BotCommand { Command = "delete_selected_file", Description = "Delete a specific file" },
            new BotCommand { Command = "delete_all_files", Description = "Delete all files" },
            new BotCommand { Command = "how_to_use", Description = "Instructions on how to use the VPN" },
            new BotCommand { Command = "install_client", Description = "Get a link to download OpenVPN client" },
            new BotCommand { Command = "about_bot", Description = "Information about the bot" },
            new BotCommand { Command = "about_project", Description = "Information about the project" },
            new BotCommand { Command = "contacts", Description = "Developer contacts" },
            new BotCommand { Command = "change_language", Description = "Change your language" },
        };

        var commandsRu = new[]
        {
            // new BotCommand { Command = "register", Description = "Зарегистрируйтесь для использования VPN" },
            new BotCommand { Command = "get_my_files", Description = "Получите свои файлы для подключения к VPN" },
            new BotCommand { Command = "make_new_file", Description = "Создайте новый файл для подключения к VPN" },
            new BotCommand { Command = "delete_selected_file", Description = "Удалить выбранный файл" },
            new BotCommand { Command = "delete_all_files", Description = "Удалить все файлы" },
            new BotCommand { Command = "how_to_use", Description = "Инструкция по использованию VPN" },
            new BotCommand { Command = "install_client", Description = "Ссылка на загрузку клиента OpenVPN" },
            new BotCommand { Command = "about_bot", Description = "Информация о боте" },
            new BotCommand { Command = "about_project", Description = "Информация о проекте" },
            new BotCommand { Command = "contacts", Description = "Контакты разработчика" },
            new BotCommand { Command = "change_language", Description = "Изменить язык" },
        };

        var commandsEl = new[]
        {
            // new BotCommand { Command = "register", Description = "Εγγραφείτε για να χρησιμοποιήσετε το VPN" },
            new BotCommand { Command = "get_my_files", Description = "Αποκτήστε τα αρχεία σας για σύνδεση στο VPN" },
            new BotCommand { Command = "make_new_file", Description = "Δημιουργήστε ένα νέο αρχείο για σύνδεση στο VPN" },
            new BotCommand { Command = "delete_selected_file", Description = "Διαγραφή συγκεκριμένου αρχείου" },
            new BotCommand { Command = "delete_all_files", Description = "Διαγραφή όλων των αρχείων" },
            new BotCommand { Command = "how_to_use", Description = "Οδηγίες χρήσης VPN" },
            new BotCommand { Command = "install_client", Description = "Λήψη του OpenVPN client" },
            new BotCommand { Command = "about_bot", Description = "Πληροφορίες για το bot" },
            new BotCommand { Command = "about_project", Description = "Πληροφορίες για το έργο" },
            new BotCommand { Command = "contacts", Description = "Στοιχεία επικοινωνίας του προγραμματιστή" },
            new BotCommand { Command = "change_language", Description = "Αλλάξτε τη γλώσσα σας" },
        };

        await _botClient.SetMyCommands(commandsEn, languageCode: "en");
        await _botClient.SetMyCommands(commandsRu, languageCode: "ru");
        await _botClient.SetMyCommands(commandsEl, languageCode: "el");
        
        return await _botClient.SendMessage(
            chatId: msg.Chat.Id,
            text: "\u2705 All commands have been successfully registered...",
            replyMarkup: new ReplyKeyboardRemove()
        );
    }
}