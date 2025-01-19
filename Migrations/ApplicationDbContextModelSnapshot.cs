﻿// <auto-generated />
using System;
using DataGateVPNBotV1.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataGateVPNBotV1.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("xgb_rackotpg")
                .HasAnnotation("ProductVersion", "6.0.36")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DataGateVPNBotV1.Models.IncomingMessageLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("FileId")
                        .HasColumnType("text");

                    b.Property<string>("FileName")
                        .HasColumnType("text");

                    b.Property<string>("FilePath")
                        .HasColumnType("text");

                    b.Property<long?>("FileSize")
                        .HasColumnType("bigint");

                    b.Property<string>("FileType")
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<string>("MessageText")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("ReceivedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("TelegramId")
                        .HasColumnType("bigint");

                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("IncomingMessageLog", "xgb_rackotpg");
                });

            modelBuilder.Entity("DataGateVPNBotV1.Models.IssuedOvpnFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("CertFilePath")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<string>("CertName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<bool>("IsRevoked")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("IssuedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("IssuedTo")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("KeyFilePath")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<string>("PemFilePath")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ReqFilePath")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<long>("TelegramId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("IssuedOvpnFiles", "xgb_rackotpg");
                });

            modelBuilder.Entity("DataGateVPNBotV1.Models.LocalizationText", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<int>("Language")
                        .HasColumnType("integer");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("LocalizationTexts", "xgb_rackotpg");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Key = "BotMenu",
                            Language = 1,
                            Text = "<b><u>Bot Menu</u></b>:\n/get_my_files - get your files for connecting to the VPN\n/make_new_file - create a new file for connecting to the VPN\n/delete_selected_file - Delete a specific file\n/delete_all_files - Delete all files\n/how_to_use - receive information on how to use the VPN\n/install_client - get a link to download the OpenVPN client for connecting to the VPN\n/about_bot - receive information about this bot\n/about_project - receive information about the project\n/contacts - receive contacts developer\n/change_language - Change your language/Изменить язык/Αλλάξτε τη γλώσσα σας"
                        },
                        new
                        {
                            Id = 2,
                            Key = "BotMenu",
                            Language = 2,
                            Text = "<b><u>Μενού Bot</u></b>:\n/get_my_files - αποκτήστε τα αρχεία σας για σύνδεση στο VPN\n/make_new_file - δημιουργήστε ένα νέο αρχείο για σύνδεση στο VPN\n/delete_selected_file - Διαγραφή συγκεκριμένου αρχείου\n/delete_all_files - Διαγραφή όλων των αρχείων\n/how_to_use - λάβετε πληροφορίες για τη χρήση του VPN\n/install_client - λάβετε σύνδεσμο για λήψη του OpenVPN client\n/about_bot - λάβετε πληροφορίες για αυτό το bot\n/about_project - λάβετε πληροφορίες για το έργο\n/contacts - λάβετε στοιχεία επικοινωνίας του προγραμματιστή\n/change_language - Change your language/Изменить язык/Αλλάξτε τη γλώσσα σας"
                        },
                        new
                        {
                            Id = 3,
                            Key = "BotMenu",
                            Language = 3,
                            Text = "<b><u>Меню бота</u></b>:\n/get_my_files - получите свои файлы для подключения к VPN\n/make_new_file - создайте новый файл для подключения к VPN\n/delete_selected_file - Удалить выбранный файл\n/delete_all_files - Удалить все файлы\n/how_to_use - получите информацию о том, как использовать VPN\n/install_client - получите ссылку для загрузки клиента OpenVPN\n/about_bot - информация об этом боте\n/about_project - информация о проекте\n/contacts - контакты разработчика\n/change_language - Change your language/Изменить язык/Αλλάξτε τη γλώσσα σας"
                        },
                        new
                        {
                            Id = 4,
                            Key = "AboutBot",
                            Language = 1,
                            Text = "This bot helps users manage their VPN connections easily. With this bot, you can:\n- Get detailed instructions on how to use a VPN.\n- Register and obtain configuration files for VPN access.\n- Create new VPN configuration files if needed.\n- Download the OpenVPN client for seamless connection.\n- Learn about the bot's developer.\n\nThe bot is designed to provide quick and secure access to VPN features, ensuring user-friendly interaction and reliable support."
                        },
                        new
                        {
                            Id = 5,
                            Key = "AboutBot",
                            Language = 2,
                            Text = "Αυτό το bot βοηθά τους χρήστες να διαχειρίζονται εύκολα τις συνδέσεις VPN τους. Με αυτό το bot, μπορείτε:\n- Να λάβετε λεπτομερείς οδηγίες για τη χρήση VPN.\n- Να εγγραφείτε και να αποκτήσετε αρχεία διαμόρφωσης για πρόσβαση στο VPN.\n- Να δημιουργήσετε νέα αρχεία διαμόρφωσης VPN αν χρειάζεται.\n- Να κατεβάσετε τον OpenVPN client για ομαλή σύνδεση.\n- Να μάθετε για τον προγραμματιστή του bot.\n\nΤο bot είναι σχεδιασμένο για να παρέχει γρήγορη και ασφαλή πρόσβαση στις δυνατότητες του VPN, εξασφαλίζοντας φιλική προς το χρήστη αλληλεπίδραση και αξιόπιστη υποστήριξη."
                        },
                        new
                        {
                            Id = 6,
                            Key = "AboutBot",
                            Language = 3,
                            Text = "Этот бот помогает пользователям легко управлять подключениями VPN. С его помощью вы можете:\n- Получить подробные инструкции по использованию VPN.\n- Зарегистрироваться и получить файлы конфигурации для доступа к VPN.\n- Создать новые файлы конфигурации VPN при необходимости.\n- Скачать клиент OpenVPN для удобного подключения.\n- Узнать о разработчике бота.\n\nБот создан для быстрого и безопасного доступа к возможностям VPN, обеспечивая удобное взаимодействие с пользователем и надежную поддержку."
                        },
                        new
                        {
                            Id = 7,
                            Key = "Registered",
                            Language = 1,
                            Text = "You have successfully registered for VPN access!"
                        },
                        new
                        {
                            Id = 8,
                            Key = "Registered",
                            Language = 2,
                            Text = "Έχετε εγγραφεί με επιτυχία για πρόσβαση στο VPN!"
                        },
                        new
                        {
                            Id = 9,
                            Key = "Registered",
                            Language = 3,
                            Text = "Вы успешно зарегистрировались для доступа к VPN!"
                        },
                        new
                        {
                            Id = 10,
                            Key = "HowToUseVPN",
                            Language = 1,
                            Text = "To use the VPN, follow these steps:\n1. Register:\nUse the /register command to register and enable VPN access.\n\n2. Get Configuration Files:\nAfter registration, use the /get_my_files command to download your personal configuration files for OpenVPN.\n\n3. Install OpenVPN Client:\nUse the /install_client command to get a link to download the official OpenVPN client.\nInstall the OpenVPN client on your device (Windows, macOS, Linux, or mobile).\n\n4. Load Configuration Files:\nOpen the OpenVPN client and import the configuration file you downloaded from the bot.\n\n5. Connect to VPN:\nStart the OpenVPN client and select the imported configuration. Click 'Connect' to establish a secure connection."
                        },
                        new
                        {
                            Id = 11,
                            Key = "HowToUseVPN",
                            Language = 2,
                            Text = "Για να χρησιμοποιήσετε το VPN, ακολουθήστε αυτά τα βήματα:\n1. Εγγραφή:\nΧρησιμοποιήστε την εντολή /register για να εγγραφείτε και να ενεργοποιήσετε την πρόσβαση VPN.\n\n2. Λήψη αρχείων διαμόρφωσης:\nΜετά την εγγραφή, χρησιμοποιήστε την εντολή /get_my_files για να κατεβάσετε τα προσωπικά σας αρχεία διαμόρφωσης για το OpenVPN.\n\n3. Εγκατάσταση OpenVPN Client:\nΧρησιμοποιήστε την εντολή /install_client για να λάβετε σύνδεσμο για λήψη του επίσημου OpenVPN client.\nΕγκαταστήστε τον OpenVPN client στη συσκευή σας (Windows, macOS, Linux ή κινητό).\n\n4. Φόρτωση αρχείων διαμόρφωσης:\nΑνοίξτε τον OpenVPN client και εισαγάγετε το αρχείο διαμόρφωσης που κατεβάσατε από το bot.\n\n5. Σύνδεση με VPN:\nΞεκινήστε τον OpenVPN client, επιλέξτε τη διαμόρφωση που εισαγάγατε και πατήστε 'Σύνδεση' για να δημιουργήσετε μια ασφαλή σύνδεση."
                        },
                        new
                        {
                            Id = 12,
                            Key = "HowToUseVPN",
                            Language = 3,
                            Text = "Для использования VPN выполните следующие шаги:\n1. Регистрация:\nИспользуйте команду /register для регистрации и активации доступа к VPN.\n\n2. Получение файлов конфигурации:\nПосле регистрации используйте команду /get_my_files для загрузки ваших личных конфигурационных файлов для OpenVPN.\n\n3. Установка клиента OpenVPN:\nИспользуйте команду /install_client, чтобы получить ссылку на загрузку официального клиента OpenVPN. Установите клиент OpenVPN на ваше устройство (Windows, macOS, Linux или мобильное устройство).\n\n4. Загрузка файлов конфигурации:\nОткройте клиент OpenVPN и импортируйте файл конфигурации, который вы загрузили из бота.\n\n5. Подключение к VPN:\nЗапустите клиент OpenVPN, выберите импортированную конфигурацию и нажмите 'Подключиться', чтобы установить безопасное соединение."
                        },
                        new
                        {
                            Id = 13,
                            Key = "ChoosePlatform",
                            Language = 1,
                            Text = "Choose your platform to download the OpenVPN client or learn more about what OpenVPN is."
                        },
                        new
                        {
                            Id = 14,
                            Key = "ChoosePlatform",
                            Language = 2,
                            Text = "Επιλέξτε την πλατφόρμα σας για να κατεβάσετε τον OpenVPN client ή να μάθετε περισσότερα για το τι είναι το OpenVPN."
                        },
                        new
                        {
                            Id = 15,
                            Key = "ChoosePlatform",
                            Language = 3,
                            Text = "Выберите свою платформу, чтобы скачать клиент OpenVPN или узнать больше о том, что такое OpenVPN."
                        },
                        new
                        {
                            Id = 16,
                            Key = "ClientConfigCreated",
                            Language = 1,
                            Text = "Client configuration created successfully in UpdateHandler."
                        },
                        new
                        {
                            Id = 17,
                            Key = "ClientConfigCreated",
                            Language = 2,
                            Text = "Η διαμόρφωση πελάτη δημιουργήθηκε με επιτυχία στο UpdateHandler."
                        },
                        new
                        {
                            Id = 18,
                            Key = "ClientConfigCreated",
                            Language = 3,
                            Text = "Конфигурация клиента успешно создана в UpdateHandler."
                        },
                        new
                        {
                            Id = 19,
                            Key = "HereIsConfig",
                            Language = 1,
                            Text = "Here is your OpenVPN configuration file."
                        },
                        new
                        {
                            Id = 20,
                            Key = "HereIsConfig",
                            Language = 2,
                            Text = "Εδώ είναι το αρχείο διαμόρφωσης OpenVPN σας."
                        },
                        new
                        {
                            Id = 21,
                            Key = "HereIsConfig",
                            Language = 3,
                            Text = "Вот ваш файл конфигурации OpenVPN."
                        },
                        new
                        {
                            Id = 22,
                            Key = "DeveloperContacts",
                            Language = 1,
                            Text = "📞 **Developer Contacts** 📞\n\nIf you have any questions, suggestions, or need assistance, feel free to contact me:\n\n- **Telegram**: [Contact me](https://t.me/KolganovIvan)\n- **Email**: imkolganov@gmail.com\n- **GitHub**: [Profile](https://github.com/IMKolganov)\n\nI am always happy to help and hear your feedback! 😊"
                        },
                        new
                        {
                            Id = 23,
                            Key = "DeveloperContacts",
                            Language = 2,
                            Text = "📞 **Επαφές Προγραμματιστή** 📞\n\nΑν έχετε οποιεσδήποτε ερωτήσεις, προτάσεις ή χρειάζεστε βοήθεια, μη διστάσετε να επικοινωνήσετε μαζί μου:\n\n- **Telegram**: [Επικοινωνήστε μαζί μου](https://t.me/KolganovIvan)\n- **Email**: imkolganov@gmail.com\n- **GitHub**: [Προφίλ](https://github.com/IMKolganov)\n\nΕίμαι πάντα χαρούμενος να βοηθήσω και να ακούσω τα σχόλιά σας! 😊"
                        },
                        new
                        {
                            Id = 24,
                            Key = "DeveloperContacts",
                            Language = 3,
                            Text = "📞 **Контакты разработчика** 📞\n\nЕсли у вас есть вопросы, предложения или нужна помощь, не стесняйтесь связаться со мной:\n\n- **Telegram**: [Связаться со мной](https://t.me/KolganovIvan)\n- **Email**: imkolganov@gmail.com\n- **GitHub**: [Профиль](https://github.com/IMKolganov)\n\nЯ всегда рад помочь и выслушать ваши отзывы! 😊"
                        },
                        new
                        {
                            Id = 25,
                            Key = "AboutProject",
                            Language = 1,
                            Text = "🌐 **About this project** 🌐\n\nThis project is created with love and care, primarily for the people closest to me. 💖\n\nIt runs on a humble Raspberry Pi, which hums softly with its tiny fan, working tirelessly 24/7 next to my desk. 🛠️📡\n\nThanks to this little device, my loved ones can enjoy unrestricted access to the vast world of the internet, no matter where they are. 🌍\n\nFor me, it's not just a project, but a way to ensure that the people I care about most always stay connected and free online. ✨"
                        },
                        new
                        {
                            Id = 26,
                            Key = "AboutProject",
                            Language = 2,
                            Text = "🌐 **Σχετικά με αυτό το έργο** 🌐\n\nΑυτό το έργο δημιουργήθηκε με αγάπη και φροντίδα, κυρίως για τα πιο κοντινά μου άτομα. 💖\n\nΛειτουργεί σε ένα απλό Raspberry Pi, το οποίο δουλεύει αθόρυβα με το μικρό του ανεμιστήρα, ακούραστα 24/7 δίπλα στο γραφείο μου. 🛠️📡\n\nΧάρη σε αυτήν τη μικρή συσκευή, οι αγαπημένοι μου μπορούν να απολαμβάνουν απεριόριστη πρόσβαση στον τεράστιο κόσμο του διαδικτύου, ανεξάρτητα από το πού βρίσκονται. 🌍\n\nΓια μένα, δεν είναι απλώς ένα έργο, αλλά ένας τρόπος να διασφαλίσω ότι τα άτομα που με ενδιαφέρουν περισσότερο θα παραμείνουν πάντα συνδεδεμένα και ελεύθερα στο διαδίκτυο. ✨"
                        },
                        new
                        {
                            Id = 27,
                            Key = "AboutProject",
                            Language = 3,
                            Text = "🌐 **О проекте** 🌐\n\nЭтот проект создан с любовью и заботой, главным образом для самых близких мне людей. 💖\n\nОн работает на скромном Raspberry Pi, который тихо жужжит своим маленьким вентилятором, неустанно трудясь 24/7 рядом с моим столом. 🛠️📡\n\nБлагодаря этому небольшому устройству, мои близкие могут наслаждаться неограниченным доступом к огромному миру интернета, где бы они ни находились. 🌍\n\nДля меня это не просто проект, а способ убедиться, что люди, о которых я больше всего забочусь, всегда остаются на связи и свободны в интернете. ✨"
                        },
                        new
                        {
                            Id = 31,
                            Key = "ChangeLanguage",
                            Language = 1,
                            Text = "/change_language - Change your language"
                        },
                        new
                        {
                            Id = 32,
                            Key = "ChangeLanguage",
                            Language = 2,
                            Text = "/change_language - Αλλάξτε τη γλώσσα σας"
                        },
                        new
                        {
                            Id = 33,
                            Key = "ChangeLanguage",
                            Language = 3,
                            Text = "/change_language - Изменить язык"
                        },
                        new
                        {
                            Id = 34,
                            Key = "SuccessChangeLanguage",
                            Language = 1,
                            Text = "✅ You have successfully changed your language to English!"
                        },
                        new
                        {
                            Id = 35,
                            Key = "SuccessChangeLanguage",
                            Language = 2,
                            Text = "✅ Έχετε αλλάξει τη γλώσσα σας σε Ελληνικά!"
                        },
                        new
                        {
                            Id = 36,
                            Key = "SuccessChangeLanguage",
                            Language = 3,
                            Text = "✅ Вы успешно сменили язык на Русский!"
                        },
                        new
                        {
                            Id = 37,
                            Key = "FilesNotFoundError",
                            Language = 1,
                            Text = "You have no files, but you can create them by selecting the /make_new_file command."
                        },
                        new
                        {
                            Id = 38,
                            Key = "FilesNotFoundError",
                            Language = 3,
                            Text = "У вас нет файлов, но вы можете создать их, выбрав команду /make_new_file."
                        },
                        new
                        {
                            Id = 39,
                            Key = "FilesNotFoundError",
                            Language = 2,
                            Text = "Δεν έχετε αρχεία, αλλά μπορείτε να τα δημιουργήσετε επιλέγοντας την εντολή /make_new_file."
                        },
                        new
                        {
                            Id = 40,
                            Key = "MaxConfigError",
                            Language = 1,
                            Text = "Maximum limit of 10 configurations for your devices has been reached. Cannot create more files."
                        },
                        new
                        {
                            Id = 41,
                            Key = "MaxConfigError",
                            Language = 3,
                            Text = "Достигнут максимальный лимит в 10 конфигураций для ваших устройств. Невозможно создать новые файлы."
                        },
                        new
                        {
                            Id = 42,
                            Key = "MaxConfigError",
                            Language = 2,
                            Text = "Έχει επιτευχθεί το μέγιστο όριο 10 διαμορφώσεων για τις συσκευές σας. Δεν μπορείτε να δημιουργήσετε περισσότερα αρχεία."
                        });
                });

            modelBuilder.Entity("DataGateVPNBotV1.Models.TelegramUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("FirstName")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("LastName")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<DateTime>("RegisteredAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("TelegramId")
                        .HasColumnType("bigint");

                    b.Property<string>("Username")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.ToTable("TelegramUsers", "xgb_rackotpg");
                });

            modelBuilder.Entity("DataGateVPNBotV1.Models.UserLanguagePreference", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("PreferredLanguage")
                        .HasColumnType("integer");

                    b.Property<long>("TelegramId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("UserLanguagePreferences", "xgb_rackotpg");
                });
#pragma warning restore 612, 618
        }
    }
}
