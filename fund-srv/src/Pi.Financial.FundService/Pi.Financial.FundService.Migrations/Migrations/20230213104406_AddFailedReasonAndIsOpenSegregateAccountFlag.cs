using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.Financial.FundService.Migrations.Migrations
{
    public partial class AddFailedReasonAndIsOpenSegregateAccountFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "failed_reason",
                table: "fund_account_opening_state",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "is_open_segregate_account",
                table: "fund_account_opening_state",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "failed_reason",
                table: "fund_account_opening_state");

            migrationBuilder.DropColumn(
                name: "is_open_segregate_account",
                table: "fund_account_opening_state");
        }
    }
}
