using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.User.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class InitializedDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_infos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    customerid = table.Column<string>(name: "customer_id", type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_infos", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cust_codes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    customercode = table.Column<string>(name: "customer_code", type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    userinfoid = table.Column<Guid>(name: "user_info_id", type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cust_codes", x => x.id);
                    table.ForeignKey(
                        name: "fk_cust_codes_user_infos_user_info_id",
                        column: x => x.userinfoid,
                        principalTable: "user_infos",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "devices",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    deviceid = table.Column<Guid>(name: "device_id", type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    devicetoken = table.Column<string>(name: "device_token", type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deviceidentifier = table.Column<string>(name: "device_identifier", type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    language = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    userinfoid = table.Column<Guid>(name: "user_info_id", type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_devices", x => x.id);
                    table.ForeignKey(
                        name: "fk_devices_user_infos_user_info_id",
                        column: x => x.userinfoid,
                        principalTable: "user_infos",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "trading_accounts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tradingaccountid = table.Column<string>(name: "trading_account_id", type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    userinfoid = table.Column<Guid>(name: "user_info_id", type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trading_accounts", x => x.id);
                    table.ForeignKey(
                        name: "fk_trading_accounts_user_infos_user_info_id",
                        column: x => x.userinfoid,
                        principalTable: "user_infos",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "notification_preferences",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    important = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    order = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    portfolio = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    wallet = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    market = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    deviceforeignkey = table.Column<Guid>(name: "device_foreign_key", type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    userinfoid = table.Column<Guid>(name: "user_info_id", type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notification_preferences", x => x.id);
                    table.ForeignKey(
                        name: "fk_notification_preferences_devices_device_foreign_key",
                        column: x => x.deviceforeignkey,
                        principalTable: "devices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_notification_preferences_user_infos_user_info_id",
                        column: x => x.userinfoid,
                        principalTable: "user_infos",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_cust_codes_user_info_id",
                table: "cust_codes",
                column: "user_info_id");

            migrationBuilder.CreateIndex(
                name: "ix_devices_user_info_id",
                table: "devices",
                column: "user_info_id");

            migrationBuilder.CreateIndex(
                name: "ix_notification_preferences_device_foreign_key",
                table: "notification_preferences",
                column: "device_foreign_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_notification_preferences_user_info_id",
                table: "notification_preferences",
                column: "user_info_id");

            migrationBuilder.CreateIndex(
                name: "ix_trading_accounts_user_info_id",
                table: "trading_accounts",
                column: "user_info_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cust_codes");

            migrationBuilder.DropTable(
                name: "notification_preferences");

            migrationBuilder.DropTable(
                name: "trading_accounts");

            migrationBuilder.DropTable(
                name: "devices");

            migrationBuilder.DropTable(
                name: "user_infos");
        }
    }
}
