using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.User.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class FixExternalAccForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_external_accounts_trade_accounts_id",
                table: "external_accounts");

            migrationBuilder.AddForeignKey(
                name: "fk_external_accounts_trade_accounts_trade_account_id",
                table: "external_accounts",
                column: "trade_account_id",
                principalTable: "trade_accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_external_accounts_trade_accounts_trade_account_id",
                table: "external_accounts");

            migrationBuilder.AddForeignKey(
                name: "fk_external_accounts_trade_accounts_id",
                table: "external_accounts",
                column: "id",
                principalTable: "trade_accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
