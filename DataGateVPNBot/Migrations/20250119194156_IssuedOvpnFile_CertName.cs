using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataGateVPNBot.Migrations
{
    public partial class IssuedOvpnFile_CertName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CertName",
                schema: "xgb_botvpndev",
                table: "IssuedOvpnFiles",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CertName",
                schema: "xgb_botvpndev",
                table: "IssuedOvpnFiles");
        }
    }
}
