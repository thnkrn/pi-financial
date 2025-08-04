using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.SetService.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexForSblOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_sbl_orders_symbol",
                table: "sbl_orders",
                column: "symbol");

            migrationBuilder.CreateIndex(
                name: "ix_sbl_orders_trading_account_no",
                table: "sbl_orders",
                column: "trading_account_no");

            migrationBuilder.CreateIndex(
                name: "ix_sbl_instruments_symbol",
                table: "sbl_instruments",
                column: "symbol");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_sbl_orders_symbol",
                table: "sbl_orders");

            migrationBuilder.DropIndex(
                name: "ix_sbl_orders_trading_account_no",
                table: "sbl_orders");

            migrationBuilder.DropIndex(
                name: "ix_sbl_instruments_symbol",
                table: "sbl_instruments");
        }
    }
}
