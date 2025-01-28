using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataGateVPNBotV1.Migrations
{
    /// <inheritdoc />
    public partial class OpenVpnUserStatistic_SessionId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TelegramId",
                schema: "xgb_botvpnprod",
                table: "OpenVpnUserStatistics");

            migrationBuilder.AlterColumn<string>(
                name: "RealAddress",
                schema: "xgb_botvpnprod",
                table: "OpenVpnUserStatistics",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "CommonName",
                schema: "xgb_botvpnprod",
                table: "OpenVpnUserStatistics",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                schema: "xgb_botvpnprod",
                table: "OpenVpnUserStatistics",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "SessionId",
                schema: "xgb_botvpnprod",
                table: "OpenVpnUserStatistics",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdated",
                schema: "xgb_botvpnprod",
                table: "OpenVpnUserStatistics");

            migrationBuilder.DropColumn(
                name: "SessionId",
                schema: "xgb_botvpnprod",
                table: "OpenVpnUserStatistics");

            migrationBuilder.AlterColumn<string>(
                name: "RealAddress",
                schema: "xgb_botvpnprod",
                table: "OpenVpnUserStatistics",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "CommonName",
                schema: "xgb_botvpnprod",
                table: "OpenVpnUserStatistics",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<long>(
                name: "TelegramId",
                schema: "xgb_botvpnprod",
                table: "OpenVpnUserStatistics",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
