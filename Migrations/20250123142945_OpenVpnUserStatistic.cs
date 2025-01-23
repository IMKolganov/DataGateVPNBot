using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataGateVPNBotV1.Migrations
{
    /// <inheritdoc />
    public partial class OpenVpnUserStatistic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OpenVpnUserStatistics",
                schema: "xgb_rackotpg",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TelegramId = table.Column<long>(type: "bigint", nullable: false),
                    CommonName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RealAddress = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BytesReceived = table.Column<long>(type: "bigint", nullable: false),
                    BytesSent = table.Column<long>(type: "bigint", nullable: false),
                    ConnectedSince = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenVpnUserStatistics", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OpenVpnUserStatistics",
                schema: "xgb_rackotpg");
        }
    }
}
