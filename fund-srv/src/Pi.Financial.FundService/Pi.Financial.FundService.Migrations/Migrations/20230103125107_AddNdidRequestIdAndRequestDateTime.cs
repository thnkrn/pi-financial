using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.Financial.FundService.Migrations.Migrations
{
    public partial class AddNdidRequestIdAndRequestDateTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ndid_date_time",
                table: "fund_account_opening_state",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ndid_request_id",
                table: "fund_account_opening_state",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ndid_date_time",
                table: "fund_account_opening_state");

            migrationBuilder.DropColumn(
                name: "ndid_request_id",
                table: "fund_account_opening_state");
        }
    }
}
