using DataGateVPNBotV1.Models;
using DataGateVPNBotV1.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace DataGateVPNBotV1.Contexts;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<TelegramUser> TelegramUsers { get; set; }
    public DbSet<IssuedOvpnFile> IssuedOvpnFiles { get; set; } = null!;
    public DbSet<UserLanguagePreference> UserLanguagePreferences { get; set; } = null!;
    public DbSet<LocalizationText> LocalizationTexts { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TelegramUser>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TelegramId).IsRequired();
            entity.Property(e => e.Username).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
        });
        
        modelBuilder.Entity<IssuedOvpnFile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.IssuedAt).IsRequired();
            entity.Property(e => e.IssuedTo).IsRequired().HasMaxLength(255);
        });
        modelBuilder.Entity<UserLanguagePreference>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TelegramId).IsRequired();
            entity.Property(e => e.PreferredLanguage).IsRequired()
                .HasConversion<int>(); 
        });
        modelBuilder.Entity<LocalizationText>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Key)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.Language)
                .IsRequired().HasConversion<int>();
            entity.Property(e => e.Text)
                .IsRequired();
        });
        modelBuilder.Entity<LocalizationText>().HasData(
            // Bot menu
            new LocalizationText { Id = 1, Key = "BotMenu", Language = Language.English, Text = "<b><u>Bot Menu</u></b>:\n/register - register to use the VPN\n/get_my_files - get your files for connecting to the VPN\n/make_new_file - create a new file for connecting to the VPN\n/how_to_use - receive information on how to use the VPN\n/install_client - get a link to download the OpenVPN client for connecting to the VPN\n/about_bot - receive information about this bot\n/about_project - receive information about the project\n/contacts - receive contacts developer" },
            new LocalizationText { Id = 2, Key = "BotMenu", Language = Language.Greek, Text = "<b><u>Μενού Bot</u></b>:\n/register - εγγραφείτε για να χρησιμοποιήσετε το VPN\n/get_my_files - αποκτήστε τα αρχεία σας για σύνδεση στο VPN\n/make_new_file - δημιουργήστε ένα νέο αρχείο για σύνδεση στο VPN\n/how_to_use - λάβετε πληροφορίες για τη χρήση του VPN\n/install_client - λάβετε σύνδεσμο για λήψη του OpenVPN client\n/about_bot - λάβετε πληροφορίες για αυτό το bot\n/about_project - λάβετε πληροφορίες για το έργο\n/contacts - λάβετε στοιχεία επικοινωνίας του προγραμματιστή" },
            new LocalizationText { Id = 3, Key = "BotMenu", Language = Language.Russian, Text = "<b><u>Меню бота</u></b>:\n/register - зарегистрируйтесь для использования VPN\n/get_my_files - получите свои файлы для подключения к VPN\n/make_new_file - создайте новый файл для подключения к VPN\n/how_to_use - получите информацию о том, как использовать VPN\n/install_client - получите ссылку для загрузки клиента OpenVPN\n/about_bot - информация об этом боте\n/about_project - информация о проекте\n/contacts - контакты разработчика" },

            // About bot
            new LocalizationText { Id = 4, Key = "AboutBot", Language = Language.English, Text = "This bot helps users manage their VPN connections easily. With this bot, you can:\n- Get detailed instructions on how to use a VPN.\n- Register and obtain configuration files for VPN access.\n- Create new VPN configuration files if needed.\n- Download the OpenVPN client for seamless connection.\n- Learn about the bot's developer.\n\nThe bot is designed to provide quick and secure access to VPN features, ensuring user-friendly interaction and reliable support." },
            new LocalizationText { Id = 5, Key = "AboutBot", Language = Language.Greek, Text = "Αυτό το bot βοηθά τους χρήστες να διαχειρίζονται εύκολα τις συνδέσεις VPN τους. Με αυτό το bot, μπορείτε:\n- Να λάβετε λεπτομερείς οδηγίες για τη χρήση VPN.\n- Να εγγραφείτε και να αποκτήσετε αρχεία διαμόρφωσης για πρόσβαση στο VPN.\n- Να δημιουργήσετε νέα αρχεία διαμόρφωσης VPN αν χρειάζεται.\n- Να κατεβάσετε τον OpenVPN client για ομαλή σύνδεση.\n- Να μάθετε για τον προγραμματιστή του bot.\n\nΤο bot είναι σχεδιασμένο για να παρέχει γρήγορη και ασφαλή πρόσβαση στις δυνατότητες του VPN, εξασφαλίζοντας φιλική προς το χρήστη αλληλεπίδραση και αξιόπιστη υποστήριξη." },
            new LocalizationText { Id = 6, Key = "AboutBot", Language = Language.Russian, Text = "Этот бот помогает пользователям легко управлять подключениями VPN. С его помощью вы можете:\n- Получить подробные инструкции по использованию VPN.\n- Зарегистрироваться и получить файлы конфигурации для доступа к VPN.\n- Создать новые файлы конфигурации VPN при необходимости.\n- Скачать клиент OpenVPN для удобного подключения.\n- Узнать о разработчике бота.\n\nБот создан для быстрого и безопасного доступа к возможностям VPN, обеспечивая удобное взаимодействие с пользователем и надежную поддержку." },

            // Successful registration
            new LocalizationText { Id = 7, Key = "Registered", Language = Language.English, Text = "You have successfully registered for VPN access!" },
            new LocalizationText { Id = 8, Key = "Registered", Language = Language.Greek, Text = "Έχετε εγγραφεί με επιτυχία για πρόσβαση στο VPN!" },
            new LocalizationText { Id = 9, Key = "Registered", Language = Language.Russian, Text = "Вы успешно зарегистрировались для доступа к VPN!" },

            // How to use VPN
            new LocalizationText { Id = 10, Key = "HowToUseVPN", Language = Language.English, Text = "To use the VPN, follow these steps:\n1. Register:\nUse the /register command to register and enable VPN access.\n\n2. Get Configuration Files:\nAfter registration, use the /get_my_files command to download your personal configuration files for OpenVPN.\n\n3. Install OpenVPN Client:\nUse the /install_client command to get a link to download the official OpenVPN client.\nInstall the OpenVPN client on your device (Windows, macOS, Linux, or mobile).\n\n4. Load Configuration Files:\nOpen the OpenVPN client and import the configuration file you downloaded from the bot.\n\n5. Connect to VPN:\nStart the OpenVPN client and select the imported configuration. Click 'Connect' to establish a secure connection." },
            new LocalizationText { Id = 11, Key = "HowToUseVPN", Language = Language.Greek, Text = "Για να χρησιμοποιήσετε το VPN, ακολουθήστε αυτά τα βήματα:\n1. Εγγραφή:\nΧρησιμοποιήστε την εντολή /register για να εγγραφείτε και να ενεργοποιήσετε την πρόσβαση VPN.\n\n2. Λήψη αρχείων διαμόρφωσης:\nΜετά την εγγραφή, χρησιμοποιήστε την εντολή /get_my_files για να κατεβάσετε τα προσωπικά σας αρχεία διαμόρφωσης για το OpenVPN.\n\n3. Εγκατάσταση OpenVPN Client:\nΧρησιμοποιήστε την εντολή /install_client για να λάβετε σύνδεσμο για λήψη του επίσημου OpenVPN client.\nΕγκαταστήστε τον OpenVPN client στη συσκευή σας (Windows, macOS, Linux ή κινητό).\n\n4. Φόρτωση αρχείων διαμόρφωσης:\nΑνοίξτε τον OpenVPN client και εισαγάγετε το αρχείο διαμόρφωσης που κατεβάσατε από το bot.\n\n5. Σύνδεση με VPN:\nΞεκινήστε τον OpenVPN client, επιλέξτε τη διαμόρφωση που εισαγάγατε και πατήστε 'Σύνδεση' για να δημιουργήσετε μια ασφαλή σύνδεση." },
            new LocalizationText { Id = 12, Key = "HowToUseVPN", Language = Language.Russian, Text = "Для использования VPN выполните следующие шаги:\n1. Регистрация:\nИспользуйте команду /register для регистрации и активации доступа к VPN.\n\n2. Получение файлов конфигурации:\nПосле регистрации используйте команду /get_my_files для загрузки ваших личных конфигурационных файлов для OpenVPN.\n\n3. Установка клиента OpenVPN:\nИспользуйте команду /install_client, чтобы получить ссылку на загрузку официального клиента OpenVPN. Установите клиент OpenVPN на ваше устройство (Windows, macOS, Linux или мобильное устройство).\n\n4. Загрузка файлов конфигурации:\nОткройте клиент OpenVPN и импортируйте файл конфигурации, который вы загрузили из бота.\n\n5. Подключение к VPN:\nЗапустите клиент OpenVPN, выберите импортированную конфигурацию и нажмите 'Подключиться', чтобы установить безопасное соединение." },

            // Additional texts
            new LocalizationText { Id = 13, Key = "ChoosePlatform", Language = Language.English, Text = "Choose your platform to download the OpenVPN client or learn more about what OpenVPN is." },
            new LocalizationText { Id = 14, Key = "ChoosePlatform", Language = Language.Greek, Text = "Επιλέξτε την πλατφόρμα σας για να κατεβάσετε τον OpenVPN client ή να μάθετε περισσότερα για το τι είναι το OpenVPN." },
            new LocalizationText { Id = 15, Key = "ChoosePlatform", Language = Language.Russian, Text = "Выберите свою платформу, чтобы скачать клиент OpenVPN или узнать больше о том, что такое OpenVPN." },

            new LocalizationText { Id = 16, Key = "ClientConfigCreated", Language = Language.English, Text = "Client configuration created successfully in UpdateHandler." },
            new LocalizationText { Id = 17, Key = "ClientConfigCreated", Language = Language.Greek, Text = "Η διαμόρφωση πελάτη δημιουργήθηκε με επιτυχία στο UpdateHandler." },
            new LocalizationText { Id = 18, Key = "ClientConfigCreated", Language = Language.Russian, Text = "Конфигурация клиента успешно создана в UpdateHandler." },

            new LocalizationText { Id = 19, Key = "HereIsConfig", Language = Language.English, Text = "Here is your OpenVPN configuration file." },
            new LocalizationText { Id = 20, Key = "HereIsConfig", Language = Language.Greek, Text = "Εδώ είναι το αρχείο διαμόρφωσης OpenVPN σας." },
            new LocalizationText { Id = 21, Key = "HereIsConfig", Language = Language.Russian, Text = "Вот ваш файл конфигурации OpenVPN." }
        );
    }
}
