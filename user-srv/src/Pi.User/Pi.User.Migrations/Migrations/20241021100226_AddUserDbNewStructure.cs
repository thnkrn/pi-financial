using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.User.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddUserDbNewStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_cust_codes_user_infos_user_info_id1",
                table: "cust_codes");

            migrationBuilder.DropForeignKey(
                name: "fk_devices_user_infos_user_info_id1",
                table: "devices");

            migrationBuilder.DropForeignKey(
                name: "fk_notification_preferences_user_infos_user_info_id1",
                table: "notification_preferences");

            migrationBuilder.DropForeignKey(
                name: "fk_trading_accounts_user_infos_user_info_id1",
                table: "trading_accounts");

            migrationBuilder.AddColumn<Guid>(
                name: "address_id",
                table: "user_infos",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "kyc_id",
                table: "user_infos",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "addresses",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    place = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    home_no = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    town = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    building = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    village = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    floor = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    soi = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    road = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sub_district = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    district = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    province = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    country = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    zip_code = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    country_code = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    province_code = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    row_version = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_addresses", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "bank_account_v2s",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    account_no = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    hashed_account_no = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    account_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    bank_code = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    branch_code = table.Column<string>(type: "varchar(8)", maxLength: 8, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    payment_token = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ats_effective_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    status = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    trade_account_bank_account_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    row_version = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bank_account_v2s", x => x.id);
                    table.ForeignKey(
                        name: "fk_bank_account_v2s_user_infos_user_id",
                        column: x => x.user_id,
                        principalTable: "user_infos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "kycs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    review_date = table.Column<DateTime>(type: "date", nullable: false),
                    expired_date = table.Column<DateTime>(type: "date", nullable: false),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    row_version = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_kycs", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "suitability_tests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    grade = table.Column<string>(type: "varchar(1)", maxLength: 1, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    score = table.Column<int>(type: "int", nullable: false),
                    version = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    review_date = table.Column<DateTime>(type: "date", nullable: false),
                    expired_date = table.Column<DateTime>(type: "date", nullable: false),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    row_version = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_suitability_tests", x => x.id);
                    table.ForeignKey(
                        name: "fk_suitability_tests_user_infos_user_id",
                        column: x => x.user_id,
                        principalTable: "user_infos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_accounts",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    user_id1 = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    row_version = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_accounts", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_accounts_user_infos_user_id",
                        column: x => x.user_id,
                        principalTable: "user_infos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_accounts_user_infos_user_id1",
                        column: x => x.user_id1,
                        principalTable: "user_infos",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "trade_accounts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    account_number = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    account_type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    account_type_code = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    exchange_market_id = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    account_status = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    credit_line = table.Column<decimal>(type: "decimal(16,2)", precision: 16, scale: 2, nullable: false),
                    credit_line_currency = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    effective_date = table.Column<DateOnly>(type: "date", nullable: true),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    marketing_id = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sale_license = table.Column<string>(type: "varchar(12)", maxLength: 12, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    open_date = table.Column<DateOnly>(type: "date", nullable: true),
                    user_account_id = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    row_version = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trade_accounts", x => x.id);
                    table.ForeignKey(
                        name: "fk_trade_accounts_user_accounts_user_account_id",
                        column: x => x.user_account_id,
                        principalTable: "user_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "external_accounts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    value = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    provider_id = table.Column<int>(type: "int", nullable: false),
                    trade_account_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    row_version = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_external_accounts", x => x.id);
                    table.ForeignKey(
                        name: "fk_external_accounts_trade_accounts_id",
                        column: x => x.id,
                        principalTable: "trade_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "trade_account_bank_accounts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    rp_type = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    payment_type = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    transaction_type = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    bank_account_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    trade_account_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    row_version = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trade_account_bank_accounts", x => x.id);
                    table.ForeignKey(
                        name: "fk_trade_account_bank_accounts_bank_account_v2s_id",
                        column: x => x.id,
                        principalTable: "bank_account_v2s",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_trade_account_bank_accounts_trade_accounts_trade_account_id",
                        column: x => x.trade_account_id,
                        principalTable: "trade_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_user_infos_address_id",
                table: "user_infos",
                column: "address_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_infos_kyc_id",
                table: "user_infos",
                column: "kyc_id");

            migrationBuilder.CreateIndex(
                name: "ix_addresses_user_id",
                table: "addresses",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_bank_account_v2s_hashed_account_no_bank_code",
                table: "bank_account_v2s",
                columns: new[] { "hashed_account_no", "bank_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_bank_account_v2s_user_id",
                table: "bank_account_v2s",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_external_accounts_trade_account_id_provider_id",
                table: "external_accounts",
                columns: new[] { "trade_account_id", "provider_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_kycs_user_id",
                table: "kycs",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_suitability_tests_user_id",
                table: "suitability_tests",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_trade_account_bank_accounts_bank_account_id",
                table: "trade_account_bank_accounts",
                column: "bank_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_trade_account_bank_accounts_trade_account_id",
                table: "trade_account_bank_accounts",
                column: "trade_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_trade_accounts_user_account_id",
                table: "trade_accounts",
                column: "user_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_accounts_user_id",
                table: "user_accounts",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_accounts_user_id1",
                table: "user_accounts",
                column: "user_id1");

            migrationBuilder.AddForeignKey(
                name: "fk_cust_codes_user_infos_user_info_id",
                table: "cust_codes",
                column: "user_info_id",
                principalTable: "user_infos",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_devices_user_infos_user_info_id",
                table: "devices",
                column: "user_info_id",
                principalTable: "user_infos",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_notification_preferences_user_infos_user_info_id",
                table: "notification_preferences",
                column: "user_info_id",
                principalTable: "user_infos",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_trading_accounts_user_infos_user_info_id",
                table: "trading_accounts",
                column: "user_info_id",
                principalTable: "user_infos",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_user_infos_addresses_address_id",
                table: "user_infos",
                column: "address_id",
                principalTable: "addresses",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_user_infos_kycs_kyc_id",
                table: "user_infos",
                column: "kyc_id",
                principalTable: "kycs",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_cust_codes_user_infos_user_info_id",
                table: "cust_codes");

            migrationBuilder.DropForeignKey(
                name: "fk_devices_user_infos_user_info_id",
                table: "devices");

            migrationBuilder.DropForeignKey(
                name: "fk_notification_preferences_user_infos_user_info_id",
                table: "notification_preferences");

            migrationBuilder.DropForeignKey(
                name: "fk_trading_accounts_user_infos_user_info_id",
                table: "trading_accounts");

            migrationBuilder.DropForeignKey(
                name: "fk_user_infos_addresses_address_id",
                table: "user_infos");

            migrationBuilder.DropForeignKey(
                name: "fk_user_infos_kycs_kyc_id",
                table: "user_infos");

            migrationBuilder.DropTable(
                name: "addresses");

            migrationBuilder.DropTable(
                name: "external_accounts");

            migrationBuilder.DropTable(
                name: "kycs");

            migrationBuilder.DropTable(
                name: "suitability_tests");

            migrationBuilder.DropTable(
                name: "trade_account_bank_accounts");

            migrationBuilder.DropTable(
                name: "bank_account_v2s");

            migrationBuilder.DropTable(
                name: "trade_accounts");

            migrationBuilder.DropTable(
                name: "user_accounts");

            migrationBuilder.DropIndex(
                name: "ix_user_infos_address_id",
                table: "user_infos");

            migrationBuilder.DropIndex(
                name: "ix_user_infos_kyc_id",
                table: "user_infos");

            migrationBuilder.DropColumn(
                name: "address_id",
                table: "user_infos");

            migrationBuilder.DropColumn(
                name: "kyc_id",
                table: "user_infos");

            migrationBuilder.AddForeignKey(
                name: "fk_cust_codes_user_infos_user_info_id1",
                table: "cust_codes",
                column: "user_info_id",
                principalTable: "user_infos",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_devices_user_infos_user_info_id1",
                table: "devices",
                column: "user_info_id",
                principalTable: "user_infos",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_notification_preferences_user_infos_user_info_id1",
                table: "notification_preferences",
                column: "user_info_id",
                principalTable: "user_infos",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_trading_accounts_user_infos_user_info_id1",
                table: "trading_accounts",
                column: "user_info_id",
                principalTable: "user_infos",
                principalColumn: "id");
        }
    }
}
