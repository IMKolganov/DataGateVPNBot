using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataGateVPNBotV1.Migrations
{
    public partial class UpdateLocalizationText : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "xgb_botvpnprod",
                table: "LocalizationTexts",
                columns: new[] { "Id", "Key", "Language", "Text" },
                values: new object[,]
                {
                    { 43, "SuccessfullyDeletedAllFile", 1, "All files have been successfully deleted." },
                    { 44, "SuccessfullyDeletedAllFile", 3, "Все файлы успешно удалены." },
                    { 45, "SuccessfullyDeletedAllFile", 2, "Όλα τα αρχεία διαγράφηκαν επιτυχώς." },
                    { 46, "ChooseFileForDelete", 1, "Please choose a file to delete." },
                    { 47, "ChooseFileForDelete", 3, "Пожалуйста, выберите файл для удаления." },
                    { 48, "ChooseFileForDelete", 2, "Παρακαλώ επιλέξτε ένα αρχείο για διαγραφή." },
                    { 49, "SuccessfullyDeletedFile", 1, "The selected file has been successfully deleted." },
                    { 50, "SuccessfullyDeletedFile", 3, "Выбранный файл был успешно удалён." },
                    { 51, "SuccessfullyDeletedFile", 2, "Το επιλεγμένο αρχείο διαγράφηκε επιτυχώς." },
                    { 52, "AboutOpenVPN", 1, "About OpenVPN" },
                    { 53, "AboutOpenVPN", 3, "О OpenVPN" },
                    { 54, "AboutOpenVPN", 2, "Σχετικά με το OpenVPN" },
                    { 55, "WhatIsRaspberryPi", 1, "What is Raspberry Pi?" },
                    { 56, "WhatIsRaspberryPi", 3, "Что такое Raspberry Pi?" },
                    { 57, "WhatIsRaspberryPi", 2, "Τι είναι το Raspberry Pi;" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "xgb_botvpnprod",
                table: "LocalizationTexts",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                schema: "xgb_botvpnprod",
                table: "LocalizationTexts",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                schema: "xgb_botvpnprod",
                table: "LocalizationTexts",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                schema: "xgb_botvpnprod",
                table: "LocalizationTexts",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                schema: "xgb_botvpnprod",
                table: "LocalizationTexts",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                schema: "xgb_botvpnprod",
                table: "LocalizationTexts",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                schema: "xgb_botvpnprod",
                table: "LocalizationTexts",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                schema: "xgb_botvpnprod",
                table: "LocalizationTexts",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                schema: "xgb_botvpnprod",
                table: "LocalizationTexts",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                schema: "xgb_botvpnprod",
                table: "LocalizationTexts",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                schema: "xgb_botvpnprod",
                table: "LocalizationTexts",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                schema: "xgb_botvpnprod",
                table: "LocalizationTexts",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                schema: "xgb_botvpnprod",
                table: "LocalizationTexts",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                schema: "xgb_botvpnprod",
                table: "LocalizationTexts",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                schema: "xgb_botvpnprod",
                table: "LocalizationTexts",
                keyColumn: "Id",
                keyValue: 57);
        }
    }
}
