using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataGateVPNBotV1.Migrations
{
    public partial class LocalizationText_HowToUseVPN : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "xgb_botvpnprod",
                table: "LocalizationTexts",
                keyColumn: "Id",
                keyValue: 10,
                column: "Text",
                value: "To use the VPN, follow these steps:\n1. Get Configuration Files:\nUse the /get_my_files command to download your personal configuration files for OpenVPN.\n\n2. Install OpenVPN Client:\nUse the /install_client command to get a link to download the official OpenVPN client.\nInstall the OpenVPN client on your device (Windows, macOS, Linux, or mobile).\n\n3. Load Configuration Files:\nOpen the OpenVPN client and import the configuration file you downloaded from the bot.\n\n4. Connect to VPN:\nStart the OpenVPN client and select the imported configuration. Click 'Connect' to establish a secure connection.");

            migrationBuilder.UpdateData(
                schema: "xgb_botvpnprod",
                table: "LocalizationTexts",
                keyColumn: "Id",
                keyValue: 11,
                column: "Text",
                value: "Για να χρησιμοποιήσετε το VPN, ακολουθήστε αυτά τα βήματα:\n1. Λήψη αρχείων διαμόρφωσης:\nΧρησιμοποιήστε την εντολή /get_my_files για να κατεβάσετε τα προσωπικά σας αρχεία διαμόρφωσης για το OpenVPN.\n\n2. Εγκατάσταση OpenVPN Client:\nΧρησιμοποιήστε την εντολή /install_client για να λάβετε σύνδεσμο για λήψη του επίσημου OpenVPN client.\nΕγκαταστήστε τον OpenVPN client στη συσκευή σας (Windows, macOS, Linux ή κινητό).\n\n3. Φόρτωση αρχείων διαμόρφωσης:\nΑνοίξτε τον OpenVPN client και εισαγάγετε το αρχείο διαμόρφωσης που κατεβάσατε από το bot.\n\n4. Σύνδεση με VPN:\nΞεκινήστε τον OpenVPN client, επιλέξτε τη διαμόρφωση που εισαγάγατε και πατήστε 'Σύνδεση' για να δημιουργήσετε μια ασφαλή σύνδεση.");

            migrationBuilder.UpdateData(
                schema: "xgb_botvpnprod",
                table: "LocalizationTexts",
                keyColumn: "Id",
                keyValue: 12,
                column: "Text",
                value: "Для использования VPN выполните следующие шаги:\n1. Получение файлов конфигурации:\nИспользуйте команду /get_my_files для загрузки ваших личных конфигурационных файлов для OpenVPN.\n\n2. Установка клиента OpenVPN:\nИспользуйте команду /install_client, чтобы получить ссылку на загрузку официального клиента OpenVPN. Установите клиент OpenVPN на ваше устройство (Windows, macOS, Linux или мобильное устройство).\n\n3. Загрузка файлов конфигурации:\nОткройте клиент OpenVPN и импортируйте файл конфигурации, который вы загрузили из бота.\n\n4. Подключение к VPN:\nЗапустите клиент OpenVPN, выберите импортированную конфигурацию и нажмите 'Подключиться', чтобы установить безопасное соединение.");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "xgb_botvpnprod",
                table: "LocalizationTexts",
                keyColumn: "Id",
                keyValue: 10,
                column: "Text",
                value: "To use the VPN, follow these steps:\n1. Register:\nUse the /register command to register and enable VPN access.\n\n2. Get Configuration Files:\nAfter registration, use the /get_my_files command to download your personal configuration files for OpenVPN.\n\n3. Install OpenVPN Client:\nUse the /install_client command to get a link to download the official OpenVPN client.\nInstall the OpenVPN client on your device (Windows, macOS, Linux, or mobile).\n\n4. Load Configuration Files:\nOpen the OpenVPN client and import the configuration file you downloaded from the bot.\n\n5. Connect to VPN:\nStart the OpenVPN client and select the imported configuration. Click 'Connect' to establish a secure connection.");

            migrationBuilder.UpdateData(
                schema: "xgb_botvpnprod",
                table: "LocalizationTexts",
                keyColumn: "Id",
                keyValue: 11,
                column: "Text",
                value: "Για να χρησιμοποιήσετε το VPN, ακολουθήστε αυτά τα βήματα:\n1. Εγγραφή:\nΧρησιμοποιήστε την εντολή /register για να εγγραφείτε και να ενεργοποιήσετε την πρόσβαση VPN.\n\n2. Λήψη αρχείων διαμόρφωσης:\nΜετά την εγγραφή, χρησιμοποιήστε την εντολή /get_my_files για να κατεβάσετε τα προσωπικά σας αρχεία διαμόρφωσης για το OpenVPN.\n\n3. Εγκατάσταση OpenVPN Client:\nΧρησιμοποιήστε την εντολή /install_client για να λάβετε σύνδεσμο για λήψη του επίσημου OpenVPN client.\nΕγκαταστήστε τον OpenVPN client στη συσκευή σας (Windows, macOS, Linux ή κινητό).\n\n4. Φόρτωση αρχείων διαμόρφωσης:\nΑνοίξτε τον OpenVPN client και εισαγάγετε το αρχείο διαμόρφωσης που κατεβάσατε από το bot.\n\n5. Σύνδεση με VPN:\nΞεκινήστε τον OpenVPN client, επιλέξτε τη διαμόρφωση που εισαγάγατε και πατήστε 'Σύνδεση' για να δημιουργήσετε μια ασφαλή σύνδεση.");

            migrationBuilder.UpdateData(
                schema: "xgb_botvpnprod",
                table: "LocalizationTexts",
                keyColumn: "Id",
                keyValue: 12,
                column: "Text",
                value: "Для использования VPN выполните следующие шаги:\n1. Регистрация:\nИспользуйте команду /register для регистрации и активации доступа к VPN.\n\n2. Получение файлов конфигурации:\nПосле регистрации используйте команду /get_my_files для загрузки ваших личных конфигурационных файлов для OpenVPN.\n\n3. Установка клиента OpenVPN:\nИспользуйте команду /install_client, чтобы получить ссылку на загрузку официального клиента OpenVPN. Установите клиент OpenVPN на ваше устройство (Windows, macOS, Linux или мобильное устройство).\n\n4. Загрузка файлов конфигурации:\nОткройте клиент OpenVPN и импортируйте файл конфигурации, который вы загрузили из бота.\n\n5. Подключение к VPN:\nЗапустите клиент OpenVPN, выберите импортированную конфигурацию и нажмите 'Подключиться', чтобы установить безопасное соединение.");
        }
    }
}
