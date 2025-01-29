using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataGateVPNBot.DataBase.Migrations
{
    public partial class IssuedOvpnFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PermFilePath",
                schema: "xgb_botvpndev",
                table: "IssuedOvpnFiles",
                newName: "ReqFilePath");

            migrationBuilder.AddColumn<bool>(
                name: "IsRevoked",
                schema: "xgb_botvpndev",
                table: "IssuedOvpnFiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PemFilePath",
                schema: "xgb_botvpndev",
                table: "IssuedOvpnFiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                schema: "xgb_botvpndev",
                table: "LocalizationTexts",
                columns: new[] { "Id", "Key", "Language", "Text" },
                values: new object[,]
                {
                    { 37, "FilesNotFoundError", 1, "You have no files, but you can create them by selecting the /make_new_file command." },
                    { 38, "FilesNotFoundError", 3, "У вас нет файлов, но вы можете создать их, выбрав команду /make_new_file." },
                    { 39, "FilesNotFoundError", 2, "Δεν έχετε αρχεία, αλλά μπορείτε να τα δημιουργήσετε επιλέγοντας την εντολή /make_new_file." },
                    { 40, "MaxConfigError", 1, "Maximum limit of 10 configurations for your devices has been reached. Cannot create more files." },
                    { 41, "MaxConfigError", 3, "Достигнут максимальный лимит в 10 конфигураций для ваших устройств. Невозможно создать новые файлы." },
                    { 42, "MaxConfigError", 2, "Έχει επιτευχθεί το μέγιστο όριο 10 διαμορφώσεων για τις συσκευές σας. Δεν μπορείτε να δημιουργήσετε περισσότερα αρχεία." }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "xgb_botvpndev",
                table: "LocalizationTexts",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                schema: "xgb_botvpndev",
                table: "LocalizationTexts",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                schema: "xgb_botvpndev",
                table: "LocalizationTexts",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                schema: "xgb_botvpndev",
                table: "LocalizationTexts",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                schema: "xgb_botvpndev",
                table: "LocalizationTexts",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                schema: "xgb_botvpndev",
                table: "LocalizationTexts",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DropColumn(
                name: "IsRevoked",
                schema: "xgb_botvpndev",
                table: "IssuedOvpnFiles");

            migrationBuilder.DropColumn(
                name: "PemFilePath",
                schema: "xgb_botvpndev",
                table: "IssuedOvpnFiles");

            migrationBuilder.RenameColumn(
                name: "ReqFilePath",
                schema: "xgb_botvpndev",
                table: "IssuedOvpnFiles",
                newName: "PermFilePath");
        }
    }
}
