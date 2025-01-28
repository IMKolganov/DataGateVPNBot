using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataGateVPNBotV1.Migrations
{
    /// <inheritdoc />
    public partial class IssuedOvpnFile_CertId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CertId",
                schema: "xgb_botvpndev",
                table: "IssuedOvpnFiles",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                schema: "xgb_botvpndev",
                table: "LocalizationTexts",
                columns: new[] { "Id", "Key", "Language", "Text" },
                values: new object[,]
                {
                    { 58, "CertCriticalError", 1, "Critical error. Something wrong with certification service. Now we stop all processing, please try again later." },
                    { 59, "CertCriticalError", 3, "Критическая ошибка. Что-то пошло не так в сервисе сертификации. Все операции остановлены, пожалуйста, попробуйте позже." },
                    { 60, "CertCriticalError", 2, "Κρίσιμο σφάλμα. Κάτι πήγε στραβά με την υπηρεσία πιστοποίησης. Τώρα σταματάμε όλες τις διαδικασίες, παρακαλώ δοκιμάστε αργότερα." }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "xgb_botvpndev",
                table: "LocalizationTexts",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                schema: "xgb_botvpndev",
                table: "LocalizationTexts",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                schema: "xgb_botvpndev",
                table: "LocalizationTexts",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DropColumn(
                name: "CertId",
                schema: "xgb_botvpndev",
                table: "IssuedOvpnFiles");
        }
    }
}
