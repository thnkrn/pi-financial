using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.Financial.FundService.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddSettlementColumnsWithChanneAndAccountType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "account_type",
                table: "fund_order_state",
                type: "varchar(255)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "channel",
                table: "fund_order_state",
                type: "varchar(255)",
                nullable: false);

            migrationBuilder.AddColumn<DateOnly>(
                name: "nav_date",
                table: "fund_order_state",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reject_reason",
                table: "fund_order_state",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "settlement_bank_account",
                table: "fund_order_state",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "settlement_bank_code",
                table: "fund_order_state",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateOnly>(
                name: "settlement_date",
                table: "fund_order_state",
                type: "date",
                nullable: true);

            migrationBuilder.RenameColumn(
                name: "allotted_volume",
                table: "fund_order_state",
                newName: "allotted_unit");

            migrationBuilder.RenameColumn(
                name: "exchange_order_id",
                table: "fund_order_state",
                newName: "amc_order_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "account_type",
                table: "fund_order_state");

            migrationBuilder.DropColumn(
                name: "channel",
                table: "fund_order_state");

            migrationBuilder.DropColumn(
                name: "nav_date",
                table: "fund_order_state");

            migrationBuilder.DropColumn(
                name: "reject_reason",
                table: "fund_order_state");

            migrationBuilder.DropColumn(
                name: "settlement_bank_account",
                table: "fund_order_state");

            migrationBuilder.DropColumn(
                name: "settlement_bank_code",
                table: "fund_order_state");

            migrationBuilder.DropColumn(
                name: "settlement_date",
                table: "fund_order_state");

            migrationBuilder.RenameColumn(
                name: "allotted_unit",
                table: "fund_order_state",
                newName: "allotted_volume");

            migrationBuilder.RenameColumn(
                name: "amc_order_id",
                table: "fund_order_state",
                newName: "exchange_order_id");
        }
    }
}
