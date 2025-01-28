using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataGateVPNBotV1.Migrations
{
    /// <inheritdoc />
    public partial class IssuedOvpnFileMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Message",
                schema: "xgb_botvpndev",
                table: "IssuedOvpnFiles",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Message",
                schema: "xgb_botvpndev",
                table: "IssuedOvpnFiles");
        }
    }
}
