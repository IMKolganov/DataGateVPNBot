using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataGateVPNBot.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class IssuedOvpnFileCertIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CertId",
                schema: "xgb_botvpndev",
                table: "IssuedOvpnFiles",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.UpdateData(
                schema: "xgb_botvpndev",
                table: "LocalizationTexts",
                keyColumn: "Id",
                keyValue: 12,
                column: "Text",
                value: "Для использования VPN выполните следующие шаги:\n1. Получение файлов конфигурации:\nИспользуйте команду /get_my_files для загрузки ваших личных конфигурационных файлов для OpenVPN.\n\n2. Установка клиента OpenVPN:\nИспользуйте команду /install_client, чтобы получить ссылку на загрузку официального клиента OpenVPN. \nУстановите клиент OpenVPN на ваше устройство (Windows, macOS, Linux или мобильное устройство).\n\n3. Загрузка файлов конфигурации:\nОткройте клиент OpenVPN и импортируйте файл конфигурации, который вы загрузили из бота.\n\n4. Подключение к VPN:\nЗапустите клиент OpenVPN, выберите импортированную конфигурацию и нажмите 'Подключиться', чтобы установить безопасное соединение.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CertId",
                schema: "xgb_botvpndev",
                table: "IssuedOvpnFiles",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.UpdateData(
                schema: "xgb_botvpndev",
                table: "LocalizationTexts",
                keyColumn: "Id",
                keyValue: 12,
                column: "Text",
                value: "Для использования VPN выполните следующие шаги:\n1. Получение файлов конфигурации:\nИспользуйте команду /get_my_files для загрузки ваших личных конфигурационных файлов для OpenVPN.\n\n2. Установка клиента OpenVPN:\nИспользуйте команду /install_client, чтобы получить ссылку на загрузку официального клиента OpenVPN. Установите клиент OpenVPN на ваше устройство (Windows, macOS, Linux или мобильное устройство).\n\n3. Загрузка файлов конфигурации:\nОткройте клиент OpenVPN и импортируйте файл конфигурации, который вы загрузили из бота.\n\n4. Подключение к VPN:\nЗапустите клиент OpenVPN, выберите импортированную конфигурацию и нажмите 'Подключиться', чтобы установить безопасное соединение.");
        }
    }
}
