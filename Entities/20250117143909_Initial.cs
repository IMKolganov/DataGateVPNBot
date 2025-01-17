using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataGateVPNBotV1.Entities
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IssuedOvpnFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TelegramId = table.Column<long>(type: "bigint", nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IssuedTo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssuedOvpnFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocalizationTexts",
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
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TelegramId = table.Column<int>(type: "integer", nullable: false),
                    PreferredLanguage = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLanguagePreferences", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "LocalizationTexts",
                columns: new[] { "Id", "Key", "Language", "Text" },
                values: new object[,]
                {
                    { 1, "BotMenu", 1, "<b><u>Bot Menu</u></b>:\n/register - register to use the VPN\n/get_my_files - get your files for connecting to the VPN\n/make_new_file - create a new file for connecting to the VPN\n/how_to_use - receive information on how to use the VPN\n/install_client - get a link to download the OpenVPN client for connecting to the VPN\n/about_bot - receive information about this bot\n/about_project - receive information about the project\n/contacts - receive contacts developer" },
                    { 2, "BotMenu", 2, "<b><u>Μενού Bot</u></b>:\n/register - εγγραφείτε για να χρησιμοποιήσετε το VPN\n/get_my_files - αποκτήστε τα αρχεία σας για σύνδεση στο VPN\n/make_new_file - δημιουργήστε ένα νέο αρχείο για σύνδεση στο VPN\n/how_to_use - λάβετε πληροφορίες για τη χρήση του VPN\n/install_client - λάβετε σύνδεσμο για λήψη του OpenVPN client\n/about_bot - λάβετε πληροφορίες για αυτό το bot\n/about_project - λάβετε πληροφορίες για το έργο\n/contacts - λάβετε στοιχεία επικοινωνίας του προγραμματιστή" },
                    { 3, "BotMenu", 3, "<b><u>Меню бота</u></b>:\n/register - зарегистрируйтесь для использования VPN\n/get_my_files - получите свои файлы для подключения к VPN\n/make_new_file - создайте новый файл для подключения к VPN\n/how_to_use - получите информацию о том, как использовать VPN\n/install_client - получите ссылку для загрузки клиента OpenVPN\n/about_bot - информация об этом боте\n/about_project - информация о проекте\n/contacts - контакты разработчика" },
                    { 4, "AboutBot", 1, "This bot helps users manage their VPN connections easily. With this bot, you can:\n- Get detailed instructions on how to use a VPN.\n- Register and obtain configuration files for VPN access.\n- Create new VPN configuration files if needed.\n- Download the OpenVPN client for seamless connection.\n- Learn about the bot's developer.\n\nThe bot is designed to provide quick and secure access to VPN features, ensuring user-friendly interaction and reliable support." },
                    { 5, "AboutBot", 2, "Αυτό το bot βοηθά τους χρήστες να διαχειρίζονται εύκολα τις συνδέσεις VPN τους. Με αυτό το bot, μπορείτε:\n- Να λάβετε λεπτομερείς οδηγίες για τη χρήση VPN.\n- Να εγγραφείτε και να αποκτήσετε αρχεία διαμόρφωσης για πρόσβαση στο VPN.\n- Να δημιουργήσετε νέα αρχεία διαμόρφωσης VPN αν χρειάζεται.\n- Να κατεβάσετε τον OpenVPN client για ομαλή σύνδεση.\n- Να μάθετε για τον προγραμματιστή του bot.\n\nΤο bot είναι σχεδιασμένο για να παρέχει γρήγορη και ασφαλή πρόσβαση στις δυνατότητες του VPN, εξασφαλίζοντας φιλική προς το χρήστη αλληλεπίδραση και αξιόπιστη υποστήριξη." },
                    { 6, "AboutBot", 3, "Этот бот помогает пользователям легко управлять подключениями VPN. С его помощью вы можете:\n- Получить подробные инструкции по использованию VPN.\n- Зарегистрироваться и получить файлы конфигурации для доступа к VPN.\n- Создать новые файлы конфигурации VPN при необходимости.\n- Скачать клиент OpenVPN для удобного подключения.\n- Узнать о разработчике бота.\n\nБот создан для быстрого и безопасного доступа к возможностям VPN, обеспечивая удобное взаимодействие с пользователем и надежную поддержку." },
                    { 7, "Registered", 1, "You have successfully registered for VPN access!" },
                    { 8, "Registered", 2, "Έχετε εγγραφεί με επιτυχία για πρόσβαση στο VPN!" },
                    { 9, "Registered", 3, "Вы успешно зарегистрировались для доступа к VPN!" },
                    { 10, "HowToUseVPN", 1, "To use the VPN, follow these steps:\n1. Register:\nUse the /register command to register and enable VPN access.\n\n2. Get Configuration Files:\nAfter registration, use the /get_my_files command to download your personal configuration files for OpenVPN.\n\n3. Install OpenVPN Client:\nUse the /install_client command to get a link to download the official OpenVPN client.\nInstall the OpenVPN client on your device (Windows, macOS, Linux, or mobile).\n\n4. Load Configuration Files:\nOpen the OpenVPN client and import the configuration file you downloaded from the bot.\n\n5. Connect to VPN:\nStart the OpenVPN client and select the imported configuration. Click 'Connect' to establish a secure connection." },
                    { 11, "HowToUseVPN", 2, "Για να χρησιμοποιήσετε το VPN, ακολουθήστε αυτά τα βήματα:\n1. Εγγραφή:\nΧρησιμοποιήστε την εντολή /register για να εγγραφείτε και να ενεργοποιήσετε την πρόσβαση VPN.\n\n2. Λήψη αρχείων διαμόρφωσης:\nΜετά την εγγραφή, χρησιμοποιήστε την εντολή /get_my_files για να κατεβάσετε τα προσωπικά σας αρχεία διαμόρφωσης για το OpenVPN.\n\n3. Εγκατάσταση OpenVPN Client:\nΧρησιμοποιήστε την εντολή /install_client για να λάβετε σύνδεσμο για λήψη του επίσημου OpenVPN client.\nΕγκαταστήστε τον OpenVPN client στη συσκευή σας (Windows, macOS, Linux ή κινητό).\n\n4. Φόρτωση αρχείων διαμόρφωσης:\nΑνοίξτε τον OpenVPN client και εισαγάγετε το αρχείο διαμόρφωσης που κατεβάσατε από το bot.\n\n5. Σύνδεση με VPN:\nΞεκινήστε τον OpenVPN client, επιλέξτε τη διαμόρφωση που εισαγάγατε και πατήστε 'Σύνδεση' για να δημιουργήσετε μια ασφαλή σύνδεση." },
                    { 12, "HowToUseVPN", 3, "Для использования VPN выполните следующие шаги:\n1. Регистрация:\nИспользуйте команду /register для регистрации и активации доступа к VPN.\n\n2. Получение файлов конфигурации:\nПосле регистрации используйте команду /get_my_files для загрузки ваших личных конфигурационных файлов для OpenVPN.\n\n3. Установка клиента OpenVPN:\nИспользуйте команду /install_client, чтобы получить ссылку на загрузку официального клиента OpenVPN. Установите клиент OpenVPN на ваше устройство (Windows, macOS, Linux или мобильное устройство).\n\n4. Загрузка файлов конфигурации:\nОткройте клиент OpenVPN и импортируйте файл конфигурации, который вы загрузили из бота.\n\n5. Подключение к VPN:\nЗапустите клиент OpenVPN, выберите импортированную конфигурацию и нажмите 'Подключиться', чтобы установить безопасное соединение." }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IssuedOvpnFiles");

            migrationBuilder.DropTable(
                name: "LocalizationTexts");

            migrationBuilder.DropTable(
                name: "TelegramUsers");

            migrationBuilder.DropTable(
                name: "UserLanguagePreferences");
        }
    }
}
