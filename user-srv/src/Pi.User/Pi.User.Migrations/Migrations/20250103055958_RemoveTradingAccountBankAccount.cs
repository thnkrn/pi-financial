using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.User.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTradingAccountBankAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "trade_account_bank_accounts");

            migrationBuilder.DropColumn(
                name: "trade_account_bank_account_id",
                table: "bank_account_v2s");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "trade_account_bank_account_id",
                table: "bank_account_v2s",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "trade_account_bank_accounts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    bank_account_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    payment_type = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    row_version = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: false),
                    rp_type = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    trade_account_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    transaction_type = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
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
                name: "ix_trade_account_bank_accounts_bank_account_id",
                table: "trade_account_bank_accounts",
                column: "bank_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_trade_account_bank_accounts_trade_account_id",
                table: "trade_account_bank_accounts",
                column: "trade_account_id");
        }
    }
}
