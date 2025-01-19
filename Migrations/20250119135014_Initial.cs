using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataGateVPNBotV1.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "xgb_rackotpg");

            migrationBuilder.CreateTable(
                name: "IncomingMessageLog",
                schema: "xgb_rackotpg",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TelegramId = table.Column<long>(type: "bigint", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    MessageText = table.Column<string>(type: "text", nullable: false),
                    FileType = table.Column<string>(type: "text", nullable: true),
                    FileId = table.Column<string>(type: "text", nullable: true),
                    FileName = table.Column<string>(type: "text", nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    FilePath = table.Column<string>(type: "text", nullable: true),
                    ReceivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomingMessageLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IssuedOvpnFiles",
                schema: "xgb_rackotpg",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TelegramId = table.Column<long>(type: "bigint", nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IssuedTo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    KeyFilePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    PermFilePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CertFilePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssuedOvpnFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocalizationTexts",
                schema: "xgb_rackotpg",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Key = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Language = table.Column<int>(type: "integer", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalizationTexts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TelegramUsers",
                schema: "xgb_rackotpg",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TelegramId = table.Column<long>(type: "bigint", nullable: false),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RegisteredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserLanguagePreferences",
                schema: "xgb_rackotpg",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TelegramId = table.Column<long>(type: "bigint", nullable: false),
                    PreferredLanguage = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLanguagePreferences", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "xgb_rackotpg",
                table: "LocalizationTexts",
                columns: new[] { "Id", "Key", "Language", "Text" },
                values: new object[,]
                {
                    { 1, "BotMenu", 1, "<b><u>Bot Menu</u></b>:\n/get_my_files - get your files for connecting to the VPN\n/make_new_file - create a new file for connecting to the VPN\n/delete_selected_file - Delete a specific file\n/delete_all_files - Delete all files\n/how_to_use - receive information on how to use the VPN\n/install_client - get a link to download the OpenVPN client for connecting to the VPN\n/about_bot - receive information about this bot\n/about_project - receive information about the project\n/contacts - receive contacts developer\n/change_language - Change your language/Изменить язык/Αλλάξτε τη γλώσσα σας" },
                    { 2, "BotMenu", 2, "<b><u>Μενού Bot</u></b>:\n/get_my_files - αποκτήστε τα αρχεία σας για σύνδεση στο VPN\n/make_new_file - δημιουργήστε ένα νέο αρχείο για σύνδεση στο VPN\n/delete_selected_file - Διαγραφή συγκεκριμένου αρχείου\n/delete_all_files - Διαγραφή όλων των αρχείων\n/how_to_use - λάβετε πληροφορίες για τη χρήση του VPN\n/install_client - λάβετε σύνδεσμο για λήψη του OpenVPN client\n/about_bot - λάβετε πληροφορίες για αυτό το bot\n/about_project - λάβετε πληροφορίες για το έργο\n/contacts - λάβετε στοιχεία επικοινωνίας του προγραμματιστή\n/change_language - Change your language/Изменить язык/Αλλάξτε τη γλώσσα σας" },
                    { 3, "BotMenu", 3, "<b><u>Меню бота</u></b>:\n/get_my_files - получите свои файлы для подключения к VPN\n/make_new_file - создайте новый файл для подключения к VPN\n/delete_selected_file - Удалить выбранный файл\n/delete_all_files - Удалить все файлы\n/how_to_use - получите информацию о том, как использовать VPN\n/install_client - получите ссылку для загрузки клиента OpenVPN\n/about_bot - информация об этом боте\n/about_project - информация о проекте\n/contacts - контакты разработчика\n/change_language - Change your language/Изменить язык/Αλλάξτε τη γλώσσα σας" },
                    { 4, "AboutBot", 1, "This bot helps users manage their VPN connections easily. With this bot, you can:\n- Get detailed instructions on how to use a VPN.\n- Register and obtain configuration files for VPN access.\n- Create new VPN configuration files if needed.\n- Download the OpenVPN client for seamless connection.\n- Learn about the bot's developer.\n\nThe bot is designed to provide quick and secure access to VPN features, ensuring user-friendly interaction and reliable support." },
                    { 5, "AboutBot", 2, "Αυτό το bot βοηθά τους χρήστες να διαχειρίζονται εύκολα τις συνδέσεις VPN τους. Με αυτό το bot, μπορείτε:\n- Να λάβετε λεπτομερείς οδηγίες για τη χρήση VPN.\n- Να εγγραφείτε και να αποκτήσετε αρχεία διαμόρφωσης για πρόσβαση στο VPN.\n- Να δημιουργήσετε νέα αρχεία διαμόρφωσης VPN αν χρειάζεται.\n- Να κατεβάσετε τον OpenVPN client για ομαλή σύνδεση.\n- Να μάθετε για τον προγραμματιστή του bot.\n\nΤο bot είναι σχεδιασμένο για να παρέχει γρήγορη και ασφαλή πρόσβαση στις δυνατότητες του VPN, εξασφαλίζοντας φιλική προς το χρήστη αλληλεπίδραση και αξιόπιστη υποστήριξη." },
                    { 6, "AboutBot", 3, "Этот бот помогает пользователям легко управлять подключениями VPN. С его помощью вы можете:\n- Получить подробные инструкции по использованию VPN.\n- Зарегистрироваться и получить файлы конфигурации для доступа к VPN.\n- Создать новые файлы конфигурации VPN при необходимости.\n- Скачать клиент OpenVPN для удобного подключения.\n- Узнать о разработчике бота.\n\nБот создан для быстрого и безопасного доступа к возможностям VPN, обеспечивая удобное взаимодействие с пользователем и надежную поддержку." },
                    { 7, "Registered", 1, "You have successfully registered for VPN access!" },
                    { 8, "Registered", 2, "Έχετε εγγραφεί με επιτυχία για πρόσβαση στο VPN!" },
                    { 9, "Registered", 3, "Вы успешно зарегистрировались для доступа к VPN!" },
                    { 10, "HowToUseVPN", 1, "To use the VPN, follow these steps:\n1. Register:\nUse the /register command to register and enable VPN access.\n\n2. Get Configuration Files:\nAfter registration, use the /get_my_files command to download your personal configuration files for OpenVPN.\n\n3. Install OpenVPN Client:\nUse the /install_client command to get a link to download the official OpenVPN client.\nInstall the OpenVPN client on your device (Windows, macOS, Linux, or mobile).\n\n4. Load Configuration Files:\nOpen the OpenVPN client and import the configuration file you downloaded from the bot.\n\n5. Connect to VPN:\nStart the OpenVPN client and select the imported configuration. Click 'Connect' to establish a secure connection." },
                    { 11, "HowToUseVPN", 2, "Για να χρησιμοποιήσετε το VPN, ακολουθήστε αυτά τα βήματα:\n1. Εγγραφή:\nΧρησιμοποιήστε την εντολή /register για να εγγραφείτε και να ενεργοποιήσετε την πρόσβαση VPN.\n\n2. Λήψη αρχείων διαμόρφωσης:\nΜετά την εγγραφή, χρησιμοποιήστε την εντολή /get_my_files για να κατεβάσετε τα προσωπικά σας αρχεία διαμόρφωσης για το OpenVPN.\n\n3. Εγκατάσταση OpenVPN Client:\nΧρησιμοποιήστε την εντολή /install_client για να λάβετε σύνδεσμο για λήψη του επίσημου OpenVPN client.\nΕγκαταστήστε τον OpenVPN client στη συσκευή σας (Windows, macOS, Linux ή κινητό).\n\n4. Φόρτωση αρχείων διαμόρφωσης:\nΑνοίξτε τον OpenVPN client και εισαγάγετε το αρχείο διαμόρφωσης που κατεβάσατε από το bot.\n\n5. Σύνδεση με VPN:\nΞεκινήστε τον OpenVPN client, επιλέξτε τη διαμόρφωση που εισαγάγατε και πατήστε 'Σύνδεση' για να δημιουργήσετε μια ασφαλή σύνδεση." },
                    { 12, "HowToUseVPN", 3, "Для использования VPN выполните следующие шаги:\n1. Регистрация:\nИспользуйте команду /register для регистрации и активации доступа к VPN.\n\n2. Получение файлов конфигурации:\nПосле регистрации используйте команду /get_my_files для загрузки ваших личных конфигурационных файлов для OpenVPN.\n\n3. Установка клиента OpenVPN:\nИспользуйте команду /install_client, чтобы получить ссылку на загрузку официального клиента OpenVPN. Установите клиент OpenVPN на ваше устройство (Windows, macOS, Linux или мобильное устройство).\n\n4. Загрузка файлов конфигурации:\nОткройте клиент OpenVPN и импортируйте файл конфигурации, который вы загрузили из бота.\n\n5. Подключение к VPN:\nЗапустите клиент OpenVPN, выберите импортированную конфигурацию и нажмите 'Подключиться', чтобы установить безопасное соединение." },
                    { 13, "ChoosePlatform", 1, "Choose your platform to download the OpenVPN client or learn more about what OpenVPN is." },
                    { 14, "ChoosePlatform", 2, "Επιλέξτε την πλατφόρμα σας για να κατεβάσετε τον OpenVPN client ή να μάθετε περισσότερα για το τι είναι το OpenVPN." },
                    { 15, "ChoosePlatform", 3, "Выберите свою платформу, чтобы скачать клиент OpenVPN или узнать больше о том, что такое OpenVPN." },
                    { 16, "ClientConfigCreated", 1, "Client configuration created successfully in UpdateHandler." },
                    { 17, "ClientConfigCreated", 2, "Η διαμόρφωση πελάτη δημιουργήθηκε με επιτυχία στο UpdateHandler." },
                    { 18, "ClientConfigCreated", 3, "Конфигурация клиента успешно создана в UpdateHandler." },
                    { 19, "HereIsConfig", 1, "Here is your OpenVPN configuration file." },
                    { 20, "HereIsConfig", 2, "Εδώ είναι το αρχείο διαμόρφωσης OpenVPN σας." },
                    { 21, "HereIsConfig", 3, "Вот ваш файл конфигурации OpenVPN." },
                    { 22, "DeveloperContacts", 1, "📞 **Developer Contacts** 📞\n\nIf you have any questions, suggestions, or need assistance, feel free to contact me:\n\n- **Telegram**: [Contact me](https://t.me/KolganovIvan)\n- **Email**: imkolganov@gmail.com\n- **GitHub**: [Profile](https://github.com/IMKolganov)\n\nI am always happy to help and hear your feedback! 😊" },
                    { 23, "DeveloperContacts", 2, "📞 **Επαφές Προγραμματιστή** 📞\n\nΑν έχετε οποιεσδήποτε ερωτήσεις, προτάσεις ή χρειάζεστε βοήθεια, μη διστάσετε να επικοινωνήσετε μαζί μου:\n\n- **Telegram**: [Επικοινωνήστε μαζί μου](https://t.me/KolganovIvan)\n- **Email**: imkolganov@gmail.com\n- **GitHub**: [Προφίλ](https://github.com/IMKolganov)\n\nΕίμαι πάντα χαρούμενος να βοηθήσω και να ακούσω τα σχόλιά σας! 😊" },
                    { 24, "DeveloperContacts", 3, "📞 **Контакты разработчика** 📞\n\nЕсли у вас есть вопросы, предложения или нужна помощь, не стесняйтесь связаться со мной:\n\n- **Telegram**: [Связаться со мной](https://t.me/KolganovIvan)\n- **Email**: imkolganov@gmail.com\n- **GitHub**: [Профиль](https://github.com/IMKolganov)\n\nЯ всегда рад помочь и выслушать ваши отзывы! 😊" },
                    { 25, "AboutProject", 1, "🌐 **About this project** 🌐\n\nThis project is created with love and care, primarily for the people closest to me. 💖\n\nIt runs on a humble Raspberry Pi, which hums softly with its tiny fan, working tirelessly 24/7 next to my desk. 🛠️📡\n\nThanks to this little device, my loved ones can enjoy unrestricted access to the vast world of the internet, no matter where they are. 🌍\n\nFor me, it's not just a project, but a way to ensure that the people I care about most always stay connected and free online. ✨" },
                    { 26, "AboutProject", 2, "🌐 **Σχετικά με αυτό το έργο** 🌐\n\nΑυτό το έργο δημιουργήθηκε με αγάπη και φροντίδα, κυρίως για τα πιο κοντινά μου άτομα. 💖\n\nΛειτουργεί σε ένα απλό Raspberry Pi, το οποίο δουλεύει αθόρυβα με το μικρό του ανεμιστήρα, ακούραστα 24/7 δίπλα στο γραφείο μου. 🛠️📡\n\nΧάρη σε αυτήν τη μικρή συσκευή, οι αγαπημένοι μου μπορούν να απολαμβάνουν απεριόριστη πρόσβαση στον τεράστιο κόσμο του διαδικτύου, ανεξάρτητα από το πού βρίσκονται. 🌍\n\nΓια μένα, δεν είναι απλώς ένα έργο, αλλά ένας τρόπος να διασφαλίσω ότι τα άτομα που με ενδιαφέρουν περισσότερο θα παραμείνουν πάντα συνδεδεμένα και ελεύθερα στο διαδίκτυο. ✨" },
                    { 27, "AboutProject", 3, "🌐 **О проекте** 🌐\n\nЭтот проект создан с любовью и заботой, главным образом для самых близких мне людей. 💖\n\nОн работает на скромном Raspberry Pi, который тихо жужжит своим маленьким вентилятором, неустанно трудясь 24/7 рядом с моим столом. 🛠️📡\n\nБлагодаря этому небольшому устройству, мои близкие могут наслаждаться неограниченным доступом к огромному миру интернета, где бы они ни находились. 🌍\n\nДля меня это не просто проект, а способ убедиться, что люди, о которых я больше всего забочусь, всегда остаются на связи и свободны в интернете. ✨" },
                    { 31, "ChangeLanguage", 1, "/change_language - Change your language" },
                    { 32, "ChangeLanguage", 2, "/change_language - Αλλάξτε τη γλώσσα σας" },
                    { 33, "ChangeLanguage", 3, "/change_language - Изменить язык" },
                    { 34, "SuccessChangeLanguage", 1, "✅ You have successfully changed your language to English!" },
                    { 35, "SuccessChangeLanguage", 2, "✅ Έχετε αλλάξει τη γλώσσα σας σε Ελληνικά!" },
                    { 36, "SuccessChangeLanguage", 3, "✅ Вы успешно сменили язык на Русский!" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncomingMessageLog",
                schema: "xgb_rackotpg");

            migrationBuilder.DropTable(
                name: "IssuedOvpnFiles",
                schema: "xgb_rackotpg");

            migrationBuilder.DropTable(
                name: "LocalizationTexts",
                schema: "xgb_rackotpg");

            migrationBuilder.DropTable(
                name: "TelegramUsers",
                schema: "xgb_rackotpg");

            migrationBuilder.DropTable(
                name: "UserLanguagePreferences",
                schema: "xgb_rackotpg");
        }
    }
}
