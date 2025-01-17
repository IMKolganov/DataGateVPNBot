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
    private readonly InputPollOption[] PollOptions = new[]
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
    
    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HandleError: {Exception}", exception);
        // Cooldown in case of network connection error
        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await (update switch
        {
            { Message: { } message }                        => OnMessage(message),
            { EditedMessage: { } message }                  => OnMessage(message),
            { CallbackQuery: { } callbackQuery }            => OnCallbackQuery(callbackQuery),
            { InlineQuery: { } inlineQuery }                => OnInlineQuery(inlineQuery),
            { ChosenInlineResult: { } chosenInlineResult }  => OnChosenInlineResult(chosenInlineResult),
            { Poll: { } poll }                              => OnPoll(poll),
            { PollAnswer: { } pollAnswer }                  => OnPollAnswer(pollAnswer),
            // ChannelPost:
            // EditedChannelPost:
            // ShippingQuery:
            // PreCheckoutQuery:
            _                                               => UnknownUpdateHandlerAsync(update)
        });
    }

    private async Task OnMessage(Message msg)
    {
        _logger.LogInformation("Receive message type: {MessageType}", msg.Type);
        if (msg.Text is not { } messageText)
            return;

        Message sentMessage = await (messageText.Split(' ')[0] switch
        {
            "/about_bot" => AboutBot(msg),
            "/how_to_use" => HowToUseVPN(msg),
            "/register" => RegisterForVPN(msg),
            // "/get_my_files" => GetMyVPNFiles(msg),
            "/make_new_file" => MakeNewVPNFile(msg),
            "/install_client" => InstallClient(msg),
            "/about_project" => AboutProject(msg),
            "/contacts" => Contacts(msg),
            
            "/photo" => SendPhoto(msg),
            "/inline_buttons" => SendInlineKeyboard(msg),
            "/keyboard" => SendReplyKeyboard(msg),
            "/remove" => RemoveKeyboard(msg),
            "/request" => RequestContactAndLocation(msg),
            "/inline_mode" => StartInlineQuery(msg),
            "/poll" => SendPoll(msg),
            "/poll_anonymous" => SendAnonymousPoll(msg),
            "/throw" => FailingHandler(msg),
            _ => Usage(msg)
        });
        _logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.Id);
    }

    async Task<Message> Usage(Message msg)
    {
        const string usage = @"
<b><u>Bot Menu</u></b>:
/register        - register to use the VPN
/get_my_files    - get your files for connecting to the VPN
/make_new_file   - create a new file for connecting to the VPN

/how_to_use      - receive information on how to use the VPN
/install_client  - get a link to download the OpenVPN client for connecting to the VPN

/about_bot       - receive information about this bot
/about_project   - receive information about the project
/contacts        - receive contacts developer
            ";
            // <b><u>Bot Menu for developer's</u></b>:
            // /photo           - send a photo
            // /inline_buttons  - send inline buttons
            // /keyboard        - send keyboard buttons
            // /remove          - remove keyboard buttons
            // /request         - request location or contact
            // /inline_mode     - send inline-mode results list
            // /poll            - send a poll
            // /poll_anonymous  - send an anonymous poll
            // /throw           - what happens if handler fails
        return await _botClient.SendMessage(msg.Chat, usage, parseMode: ParseMode.Html, replyMarkup: new ReplyKeyboardRemove());
    }
    
    async Task<Message> AboutBot(Message msg)
    {
        return await _botClient.SendMessage(
            msg.Chat,
            "This bot helps users manage their VPN connections easily. With this bot, you can:\n" +
            "- Get detailed instructions on how to use a VPN.\n" +
            "- Register and obtain configuration files for VPN access.\n" +
            "- Create new VPN configuration files if needed.\n" +
            "- Download the OpenVPN client for seamless connection.\n" +
            "- Learn about the bot's developer.\n\n" +
            "The bot is designed to provide quick and secure access to VPN features, ensuring user-friendly interaction and reliable support."
        );
    }

    public async Task<Message> RegisterForVPN(Message msg)
    {
        using var scope = _serviceProvider.CreateScope();
        var registrationService = scope.ServiceProvider.GetRequiredService<TelegramRegistrationService>();

        if (msg.From != null)
            await registrationService.RegisterUserAsync(
                telegramId: msg.From.Id,
                username: msg.From.Username,
                firstName: msg.From.FirstName,
                lastName: msg.From.LastName
            );

        return await _botClient.SendTextMessageAsync(
            chatId: msg.Chat.Id,
            text: "You have successfully registered for VPN access!"
        );
    }
    async Task<Message> HowToUseVPN(Message msg)
    {
        return await _botClient.SendMessage(
            msg.Chat,
            "To use the VPN, follow these steps:\n\n" +
            "1. **Register**:\n" +
            "   - Use the `/register` command to register and enable VPN access.\n\n" +
            "2. **Get Configuration Files**:\n" +
            "   - After registration, use the `/get_my_files` command to download your personal configuration files for OpenVPN.\n\n" +
            "3. **Install OpenVPN Client**:\n" +
            "   - Use the `/install_client` command to get a link to download the official OpenVPN client.\n" +
            "   - Install the OpenVPN client on your device (Windows, macOS, Linux, or mobile).\n\n" +
            "4. **Load Configuration Files**:\n" +
            "   - Open the OpenVPN client.\n" +
            "   - Import the configuration file you downloaded from the bot.\n\n" +
            "5. **Connect to VPN**:\n" +
            "   - Start the OpenVPN client and select the imported configuration.\n" +
            "   - Click 'Connect' to establish a secure connection.\n\n" +
            "If you face any issues, feel free to reach out using `/about_bot` to learn more about the bot or `/about_project` for contact details."
        );
    }

    async Task<Message> MakeNewVPNFile(Message msg)
    {
        
        // Generate the client configuration file
        var clientConfigFile = _openVpnClientService.CreateClientConfiguration(msg.Chat.Id.ToString(), "213.133.91.43");//todo: move;
        Console.WriteLine("Client configuration created successfully in UpdateHandler.");

        // Send the .ovpn file to the user
        await using var fileStream = new FileStream(clientConfigFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
        return await _botClient.SendDocumentAsync(
            chatId: msg.Chat.Id,
            document: InputFile.FromStream(fileStream, clientConfigFile.Name),
            caption: "Here is your OpenVPN configuration file."
        );
    }

    async Task<Message> InstallClient(Message msg)
    {
        var inlineMarkup = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithUrl("Windows", "https://openvpn.net/client-connect-vpn-for-windows/"),
                InlineKeyboardButton.WithUrl("Android", "https://play.google.com/store/apps/details?id=net.openvpn.openvpn"),
                InlineKeyboardButton.WithUrl("iPhone", "https://apps.apple.com/app/openvpn-connect/id590379981")
            },
            new[]
            {
                InlineKeyboardButton.WithUrl("About OpenVPN", "https://openvpn.net/faq/what-is-openvpn/")
            }
        });

        return await _botClient.SendMessage(
            msg.Chat,
            "Choose your platform to download the OpenVPN client or learn more about what OpenVPN is:",
            replyMarkup: inlineMarkup
        );
    }
    
    async Task<Message> AboutProject(Message msg)
    {
        var inlineMarkup = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithUrl("What is Raspberry Pi?", "https://www.raspberrypi.org/about/")
            }
        });

        return await _botClient.SendMessage(
            msg.Chat,
            "🌐 **About this project** 🌐\n\n" +
            "This project is created with love and care, primarily for the people closest to me. 💖\n\n" +
            "It runs on a humble Raspberry Pi, which hums softly with its tiny fan, working tirelessly 24/7 next to my desk. 🛠️📡\n\n" +
            "Thanks to this little device, my loved ones can enjoy unrestricted access to the vast world of the internet, no matter where they are. 🌍\n\n" +
            "For me, it's not just a project, but a way to ensure that the people I care about most always stay connected and free online. ✨",
            replyMarkup: inlineMarkup
        );
    }
    
    async Task<Message> Contacts(Message msg)
    {
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
            "📞 **Developer Contacts** 📞\n\n" +
            "If you have any questions, suggestions, or need assistance, feel free to contact me:\n\n" +
            "- **Telegram**: [Contact me](https://t.me/KolganovIvan)\n" +
            "- **Email**: imkolganov@gmail.com\n" +
            "- **GitHub**: [Profile](https://github.com/IMKolganov)\n\n" +
            "I am always happy to help and hear your feedback! 😊",
            replyMarkup: inlineMarkup
        );
    }
    
    async Task<Message> SendPhoto(Message msg)
    {
        await _botClient.SendChatAction(msg.Chat, ChatAction.UploadPhoto);
        await Task.Delay(2000); // simulate a long task
        await using var fileStream = new FileStream("Files/bot.gif", FileMode.Open, FileAccess.Read);
        return await _botClient.SendPhoto(msg.Chat, fileStream, caption: "Read https://telegrambots.github.io/book/");
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
        return await _botClient.SendPoll(msg.Chat, "Question", PollOptions, isAnonymous: false);
    }

    async Task<Message> SendAnonymousPoll(Message msg)
    {
        return await _botClient.SendPoll(chatId: msg.Chat, "Question", PollOptions);
    }

    static Task<Message> FailingHandler(Message msg)
    {
        throw new NotImplementedException("FailingHandler");
    }

    // Process Inline Keyboard callback data
    private async Task OnCallbackQuery(CallbackQuery callbackQuery)
    {
        _logger.LogInformation("Received inline keyboard callback from: {CallbackQueryId}", callbackQuery.Id);
        await _botClient.AnswerCallbackQuery(callbackQuery.Id, $"Received {callbackQuery.Data}");
        await _botClient.SendMessage(callbackQuery.Message!.Chat, $"Received {callbackQuery.Data}");
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
        var selectedOption = PollOptions[answer];
        if (pollAnswer.User != null)
            await _botClient.SendMessage(pollAnswer.User.Id, $"You've chosen: {selectedOption.Text} in poll");
    }

    private Task UnknownUpdateHandlerAsync(Update update)
    {
        _logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }
}