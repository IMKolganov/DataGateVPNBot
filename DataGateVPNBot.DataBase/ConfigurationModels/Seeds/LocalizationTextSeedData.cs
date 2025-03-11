using DataGateVPNBot.Models;
using DataGateVPNBot.Models.Enums;

namespace DataGateVPNBot.DataBase.ConfigurationModels.Seeds;

public static class LocalizationTextSeedData
{
    public static LocalizationText[] GetData() => new[]
    {
        // Bot menu
        // /register - register to use the VPN\n
        // /register - εγγραφείτε για να χρησιμοποιήσετε το VPN\n
        // /register - зарегистрируйтесь для использования VPN\n
        new LocalizationText
        {
            Id = 1, Key = "BotMenu", Language = Language.English,
            Text =
                "<b><u>Bot Menu</u></b>:\n/get_my_files - get your files for connecting to the VPN" +
                "\n/make_new_file - create a new file for connecting to the VPN" +
                "\n/delete_selected_file - Delete a specific file" +
                "\n/delete_all_files - Delete all files" +
                "\n/how_to_use - receive information on how to use the VPN" +
                "\n/install_client - get a link to download the OpenVPN client for connecting to the VPN" +
                "\n/about_bot - receive information about this bot" +
                "\n/about_project - receive information about the project" +
                "\n/contacts - receive contacts developer" +
                "\n/change_language - Change your language/Изменить язык/Αλλάξτε τη γλώσσα σας"
        },
        new LocalizationText
        {
            Id = 2, Key = "BotMenu", Language = Language.Greek,
            Text =
                "<b><u>Μενού Bot</u></b>:\n/get_my_files - αποκτήστε τα αρχεία σας για σύνδεση στο VPN" +
                "\n/make_new_file - δημιουργήστε ένα νέο αρχείο για σύνδεση στο VPN" +
                "\n/delete_selected_file - Διαγραφή συγκεκριμένου αρχείου" +
                "\n/delete_all_files - Διαγραφή όλων των αρχείων" +
                "\n/how_to_use - λάβετε πληροφορίες για τη χρήση του VPN" +
                "\n/install_client - λάβετε σύνδεσμο για λήψη του OpenVPN client" +
                "\n/about_bot - λάβετε πληροφορίες για αυτό το bot" +
                "\n/about_project - λάβετε πληροφορίες για το έργο" +
                "\n/contacts - λάβετε στοιχεία επικοινωνίας του προγραμματιστή" +
                "\n/change_language - Change your language/Изменить язык/Αλλάξτε τη γλώσσα σας"
        },
        new LocalizationText
        {
            Id = 3, Key = "BotMenu", Language = Language.Russian,
            Text =
                "<b><u>Меню бота</u></b>:\n/get_my_files - получите свои файлы для подключения к VPN" +
                "\n/make_new_file - создайте новый файл для подключения к VPN" +
                "\n/delete_selected_file - Удалить выбранный файл" +
                "\n/delete_all_files - Удалить все файлы" +
                "\n/how_to_use - получите информацию о том, как использовать VPN" +
                "\n/install_client - получите ссылку для загрузки клиента OpenVPN" +
                "\n/about_bot - информация об этом боте" +
                "\n/about_project - информация о проекте" +
                "\n/contacts - контакты разработчика" +
                "\n/change_language - Change your language/Изменить язык/Αλλάξτε τη γλώσσα σας"
        },

        // About bot
        new LocalizationText
        {
            Id = 4, Key = "AboutBot", Language = Language.English,
            Text =
                "This bot helps users manage their VPN connections easily. With this bot, you can:" +
                "\n- Get detailed instructions on how to use a VPN." +
                "\n- Register and obtain configuration files for VPN access." +
                "\n- Create new VPN configuration files if needed." +
                "\n- Download the OpenVPN client for seamless connection." +
                "\n- Learn about the bot's developer." +
                "\n\nThe bot is designed to provide quick and secure access to VPN features, ensuring user-friendly interaction and reliable support."
        },
        new LocalizationText
        {
            Id = 5, Key = "AboutBot", Language = Language.Greek,
            Text =
                "Αυτό το bot βοηθά τους χρήστες να διαχειρίζονται εύκολα τις συνδέσεις VPN τους. Με αυτό το bot, μπορείτε:" +
                "\n- Να λάβετε λεπτομερείς οδηγίες για τη χρήση VPN." +
                "\n- Να εγγραφείτε και να αποκτήσετε αρχεία διαμόρφωσης για πρόσβαση στο VPN." +
                "\n- Να δημιουργήσετε νέα αρχεία διαμόρφωσης VPN αν χρειάζεται." +
                "\n- Να κατεβάσετε τον OpenVPN client για ομαλή σύνδεση." +
                "\n- Να μάθετε για τον προγραμματιστή του bot." +
                "\n\nΤο bot είναι σχεδιασμένο για να παρέχει γρήγορη και ασφαλή πρόσβαση στις δυνατότητες του VPN, εξασφαλίζοντας φιλική προς το χρήστη αλληλεπίδραση και αξιόπιστη υποστήριξη."
        },
        new LocalizationText
        {
            Id = 6, Key = "AboutBot", Language = Language.Russian,
            Text =
                "Этот бот помогает пользователям легко управлять подключениями VPN. С его помощью вы можете:" +
                "\n- Получить подробные инструкции по использованию VPN." +
                "\n- Зарегистрироваться и получить файлы конфигурации для доступа к VPN." +
                "\n- Создать новые файлы конфигурации VPN при необходимости." +
                "\n- Скачать клиент OpenVPN для удобного подключения." +
                "\n- Узнать о разработчике бота." +
                "\n\nБот создан для быстрого и безопасного доступа к возможностям VPN, обеспечивая удобное взаимодействие с пользователем и надежную поддержку."
        },

        // Successful registration
        new LocalizationText
        {
            Id = 7, Key = "Registered", Language = Language.English,
            Text = "You have successfully registered for VPN access!"
        },
        new LocalizationText
        {
            Id = 8, Key = "Registered", Language = Language.Greek,
            Text = "Έχετε εγγραφεί με επιτυχία για πρόσβαση στο VPN!"
        },
        new LocalizationText
        {
            Id = 9, Key = "Registered", Language = Language.Russian,
            Text = "Вы успешно зарегистрировались для доступа к VPN!"
        },

        // How to use VPN
        new LocalizationText
        {
            Id = 10, Key = "HowToUseVPN", Language = Language.English,
            Text =
                "To use the VPN, follow these steps:" +
                "\n1. Get Configuration Files:" +
                "\nUse the /get_my_files command to download your personal configuration files for OpenVPN." +
                "\n\n2. Install OpenVPN Client:\nUse the /install_client command to get a link to download the official OpenVPN client." +
                "\nInstall the OpenVPN client on your device (Windows, macOS, Linux, or mobile)." +
                "\n\n3. Load Configuration Files:" +
                "\nOpen the OpenVPN client and import the configuration file you downloaded from the bot." +
                "\n\n4. Connect to VPN:" +
                "\nStart the OpenVPN client and select the imported configuration. Click 'Connect' to establish a secure connection."
        },
        new LocalizationText
        {
            Id = 11, Key = "HowToUseVPN", Language = Language.Greek,
            Text =
                "Για να χρησιμοποιήσετε το VPN, ακολουθήστε αυτά τα βήματα:" +
                "\n1. Λήψη αρχείων διαμόρφωσης:" +
                "\nΧρησιμοποιήστε την εντολή /get_my_files για να κατεβάσετε τα προσωπικά σας αρχεία διαμόρφωσης για το OpenVPN." +
                "\n\n2. Εγκατάσταση OpenVPN Client:" +
                "\nΧρησιμοποιήστε την εντολή /install_client για να λάβετε σύνδεσμο για λήψη του επίσημου OpenVPN client." +
                "\nΕγκαταστήστε τον OpenVPN client στη συσκευή σας (Windows, macOS, Linux ή κινητό)." +
                "\n\n3. Φόρτωση αρχείων διαμόρφωσης:" +
                "\nΑνοίξτε τον OpenVPN client και εισαγάγετε το αρχείο διαμόρφωσης που κατεβάσατε από το bot." +
                "\n\n4. Σύνδεση με VPN:" +
                "\nΞεκινήστε τον OpenVPN client, επιλέξτε τη διαμόρφωση που εισαγάγατε και πατήστε 'Σύνδεση' για να δημιουργήσετε μια ασφαλή σύνδεση."
        },
        new LocalizationText
        {
            Id = 12, Key = "HowToUseVPN", Language = Language.Russian,
            Text =
                "Для использования VPN выполните следующие шаги:" +
                "\n1. Получение файлов конфигурации:" +
                "\nИспользуйте команду /get_my_files для загрузки ваших личных конфигурационных файлов для OpenVPN." +
                "\n\n2. Установка клиента OpenVPN:" +
                "\nИспользуйте команду /install_client, чтобы получить ссылку на загрузку официального клиента OpenVPN. " +
                "\nУстановите клиент OpenVPN на ваше устройство (Windows, macOS, Linux или мобильное устройство)." +
                "\n\n3. Загрузка файлов конфигурации:" +
                "\nОткройте клиент OpenVPN и импортируйте файл конфигурации, который вы загрузили из бота." +
                "\n\n4. Подключение к VPN:" +
                "\nЗапустите клиент OpenVPN, выберите импортированную конфигурацию и нажмите 'Подключиться', чтобы установить безопасное соединение."
        },

        // Additional texts
        new LocalizationText
        {
            Id = 13, Key = "ChoosePlatform", Language = Language.English,
            Text = "Choose your platform to download the OpenVPN client or learn more about what OpenVPN is."
        },
        new LocalizationText
        {
            Id = 14, Key = "ChoosePlatform", Language = Language.Greek,
            Text =
                "Επιλέξτε την πλατφόρμα σας για να κατεβάσετε τον OpenVPN client ή να μάθετε περισσότερα για το τι είναι το OpenVPN."
        },
        new LocalizationText
        {
            Id = 15, Key = "ChoosePlatform", Language = Language.Russian,
            Text = "Выберите свою платформу, чтобы скачать клиент OpenVPN или узнать больше о том, что такое OpenVPN."
        },

        new LocalizationText
        {
            Id = 16, Key = "ClientConfigCreated", Language = Language.English,
            Text = "Client configuration created successfully in UpdateHandler."
        },
        new LocalizationText
        {
            Id = 17, Key = "ClientConfigCreated", Language = Language.Greek,
            Text = "Η διαμόρφωση πελάτη δημιουργήθηκε με επιτυχία στο UpdateHandler."
        },
        new LocalizationText
        {
            Id = 18, Key = "ClientConfigCreated", Language = Language.Russian,
            Text = "Конфигурация клиента успешно создана в UpdateHandler."
        },

        new LocalizationText
        {
            Id = 19, Key = "HereIsConfig", Language = Language.English,
            Text = "Here is your OpenVPN configuration file."
        },
        new LocalizationText
        {
            Id = 20, Key = "HereIsConfig", Language = Language.Greek,
            Text = "Εδώ είναι το αρχείο διαμόρφωσης OpenVPN σας."
        },
        new LocalizationText
            { Id = 21, Key = "HereIsConfig", Language = Language.Russian, Text = "Вот ваш файл конфигурации OpenVPN." },

        new LocalizationText
        {
            Id = 22, Key = "DeveloperContacts", Language = Language.English,
            Text =
                "📞 **Developer Contacts** 📞" +
                "\n\nIf you have any questions, suggestions, or need assistance, feel free to contact me:" +
                "\n\n- **Telegram**: [Contact me](https://t.me/KolganovIvan)" +
                "\n- **Email**: imkolganov@gmail.com" +
                "\n- **GitHub**: [Profile](https://github.com/IMKolganov)" +
                "\n\nI am always happy to help and hear your feedback! 😊"
        },
        new LocalizationText
        {
            Id = 23, Key = "DeveloperContacts", Language = Language.Greek,
            Text =
                "📞 **Επαφές Προγραμματιστή** 📞" +
                "\n\nΑν έχετε οποιεσδήποτε ερωτήσεις, προτάσεις ή χρειάζεστε βοήθεια, μη διστάσετε να επικοινωνήσετε μαζί μου:" +
                "\n\n- **Telegram**: [Επικοινωνήστε μαζί μου](https://t.me/KolganovIvan)" +
                "\n- **Email**: imkolganov@gmail.com" +
                "\n- **GitHub**: [Προφίλ](https://github.com/IMKolganov)" +
                "\n\nΕίμαι πάντα χαρούμενος να βοηθήσω και να ακούσω τα σχόλιά σας! 😊"
        },
        new LocalizationText
        {
            Id = 24, Key = "DeveloperContacts", Language = Language.Russian,
            Text =
                "📞 **Контакты разработчика** 📞" +
                "\n\nЕсли у вас есть вопросы, предложения или нужна помощь, не стесняйтесь связаться со мной:" +
                "\n\n- **Telegram**: [Связаться со мной](https://t.me/KolganovIvan)" +
                "\n- **Email**: imkolganov@gmail.com" +
                "\n- **GitHub**: [Профиль](https://github.com/IMKolganov)" +
                "\n\nЯ всегда рад помочь и выслушать ваши отзывы! 😊"
        },

        new LocalizationText
        {
            Id = 25, Key = "AboutProject", Language = Language.English,
            Text =
                "🌐 **About this project** 🌐\n\nThis project is created with love and care, primarily for the people closest to me. 💖\n" +
                "\nIt runs on a humble Raspberry Pi, which hums softly with its tiny fan, working tirelessly 24/7 next to my desk. 🛠️📡" +
                "\n\nThanks to this little device, my loved ones can enjoy unrestricted access to the vast world of the internet, no matter where they are. 🌍" +
                "\n\nFor me, it's not just a project, but a way to ensure that the people I care about most always stay connected and free online. ✨"
        },
        new LocalizationText
        {
            Id = 26, Key = "AboutProject", Language = Language.Greek,
            Text =
                "🌐 **Σχετικά με αυτό το έργο** 🌐\n\nΑυτό το έργο δημιουργήθηκε με αγάπη και φροντίδα, κυρίως για τα πιο κοντινά μου άτομα. 💖" +
                "\n\nΛειτουργεί σε ένα απλό Raspberry Pi, το οποίο δουλεύει αθόρυβα με το μικρό του ανεμιστήρα, ακούραστα 24/7 δίπλα στο γραφείο μου. 🛠️📡" +
                "\n\nΧάρη σε αυτήν τη μικρή συσκευή, οι αγαπημένοι μου μπορούν να απολαμβάνουν απεριόριστη πρόσβαση στον τεράστιο κόσμο του διαδικτύου, ανεξάρτητα από το πού βρίσκονται. 🌍" +
                "\n\nΓια μένα, δεν είναι απλώς ένα έργο, αλλά ένας τρόπος να διασφαλίσω ότι τα άτομα που με ενδιαφέρουν περισσότερο θα παραμείνουν πάντα συνδεδεμένα και ελεύθερα στο διαδίκτυο. ✨"
        },
        new LocalizationText
        {
            Id = 27, Key = "AboutProject", Language = Language.Russian,
            Text =
                "🌐 **О проекте** 🌐\n\nЭтот проект создан с любовью и заботой, главным образом для самых близких мне людей. 💖" +
                "\n\nОн работает на скромном Raspberry Pi, который тихо жужжит своим маленьким вентилятором, неустанно трудясь 24/7 рядом с моим столом. 🛠️📡" +
                "\n\nБлагодаря этому небольшому устройству, мои близкие могут наслаждаться неограниченным доступом к огромному миру интернета, где бы они ни находились. 🌍" +
                "\n\nДля меня это не просто проект, а способ убедиться, что люди, о которых я больше всего забочусь, всегда остаются на связи и свободны в интернете. ✨"
        },

        new LocalizationText
        {
            Id = 31, Key = "ChangeLanguage", Language = Language.English,
            Text = "/change_language - Change your language"
        },
        new LocalizationText
        {
            Id = 32, Key = "ChangeLanguage", Language = Language.Greek,
            Text = "/change_language - Αλλάξτε τη γλώσσα σας"
        },
        new LocalizationText
            { Id = 33, Key = "ChangeLanguage", Language = Language.Russian, Text = "/change_language - Изменить язык" },

        new LocalizationText
        {
            Id = 34, Key = "SuccessChangeLanguage", Language = Language.English,
            Text = "✅ You have successfully changed your language to English!"
        },
        new LocalizationText
        {
            Id = 35, Key = "SuccessChangeLanguage", Language = Language.Greek,
            Text = "✅ Έχετε αλλάξει τη γλώσσα σας σε Ελληνικά!"
        },
        new LocalizationText
        {
            Id = 36, Key = "SuccessChangeLanguage", Language = Language.Russian,
            Text = "✅ Вы успешно сменили язык на Русский!"
        },

        new LocalizationText
        {
            Id = 37, Key = "FilesNotFoundError", Language = Language.English,
            Text = "You have no files, but you can create them by selecting the /make_new_file command."
        },
        new LocalizationText
        {
            Id = 38, Key = "FilesNotFoundError", Language = Language.Russian,
            Text = "У вас нет файлов, но вы можете создать их, выбрав команду /make_new_file."
        },
        new LocalizationText
        {
            Id = 39, Key = "FilesNotFoundError", Language = Language.Greek,
            Text = "Δεν έχετε αρχεία, αλλά μπορείτε να τα δημιουργήσετε επιλέγοντας την εντολή /make_new_file."
        },

        new LocalizationText
        {
            Id = 40, Key = "MaxConfigError", Language = Language.English,
            Text = "Maximum limit of 10 configurations for your devices has been reached. Cannot create more files."
        },
        new LocalizationText
        {
            Id = 41, Key = "MaxConfigError", Language = Language.Russian,
            Text = "Достигнут максимальный лимит в 10 конфигураций для ваших устройств. Невозможно создать новые файлы."
        },
        new LocalizationText
        {
            Id = 42, Key = "MaxConfigError", Language = Language.Greek,
            Text =
                "Έχει επιτευχθεί το μέγιστο όριο 10 διαμορφώσεων για τις συσκευές σας. Δεν μπορείτε να δημιουργήσετε περισσότερα αρχεία."
        },

        new LocalizationText
        {
            Id = 43, Key = "SuccessfullyDeletedAllFile", Language = Language.English,
            Text = "All files have been successfully deleted."
        },
        new LocalizationText
        {
            Id = 44, Key = "SuccessfullyDeletedAllFile", Language = Language.Russian,
            Text = "Все файлы успешно удалены."
        },
        new LocalizationText
        {
            Id = 45, Key = "SuccessfullyDeletedAllFile", Language = Language.Greek,
            Text = "Όλα τα αρχεία διαγράφηκαν επιτυχώς."
        },

        new LocalizationText
        {
            Id = 46, Key = "ChooseFileForDelete", Language = Language.English, Text = "Please choose a file to delete."
        },
        new LocalizationText
        {
            Id = 47, Key = "ChooseFileForDelete", Language = Language.Russian,
            Text = "Пожалуйста, выберите файл для удаления."
        },
        new LocalizationText
        {
            Id = 48, Key = "ChooseFileForDelete", Language = Language.Greek,
            Text = "Παρακαλώ επιλέξτε ένα αρχείο για διαγραφή."
        },

        new LocalizationText
        {
            Id = 49, Key = "SuccessfullyDeletedFile", Language = Language.English,
            Text = "The selected file has been successfully deleted."
        },
        new LocalizationText
        {
            Id = 50, Key = "SuccessfullyDeletedFile", Language = Language.Russian,
            Text = "Выбранный файл был успешно удалён."
        },
        new LocalizationText
        {
            Id = 51, Key = "SuccessfullyDeletedFile", Language = Language.Greek,
            Text = "Το επιλεγμένο αρχείο διαγράφηκε επιτυχώς."
        },

        new LocalizationText { Id = 52, Key = "AboutOpenVPN", Language = Language.English, Text = "About OpenVPN" },
        new LocalizationText { Id = 53, Key = "AboutOpenVPN", Language = Language.Russian, Text = "О OpenVPN" },
        new LocalizationText
            { Id = 54, Key = "AboutOpenVPN", Language = Language.Greek, Text = "Σχετικά με το OpenVPN" },

        new LocalizationText
            { Id = 55, Key = "WhatIsRaspberryPi", Language = Language.English, Text = "What is Raspberry Pi?" },
        new LocalizationText
            { Id = 56, Key = "WhatIsRaspberryPi", Language = Language.Russian, Text = "Что такое Raspberry Pi?" },
        new LocalizationText
            { Id = 57, Key = "WhatIsRaspberryPi", Language = Language.Greek, Text = "Τι είναι το Raspberry Pi;" },

        new LocalizationText
        {
            Id = 58, Key = "CertCriticalError", Language = Language.English,
            Text =
                "Critical error. Something wrong with certification service. Now we stop all processing, please try again later."
        },
        new LocalizationText
        {
            Id = 59, Key = "CertCriticalError", Language = Language.Russian,
            Text =
                "Критическая ошибка. Что-то пошло не так в сервисе сертификации. Все операции остановлены, пожалуйста, попробуйте позже."
        },
        new LocalizationText
        {
            Id = 60, Key = "CertCriticalError", Language = Language.Greek,
            Text =
                "Κρίσιμο σφάλμα. Κάτι πήγε στραβά με την υπηρεσία πιστοποίησης. Τώρα σταματάμε όλες τις διαδικασίες, παρακαλώ δοκιμάστε αργότερα."
        }
    };

}