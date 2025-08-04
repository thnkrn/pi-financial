using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.User.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingColumnOnUserInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "date_of_birth",
                table: "user_infos",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "user_account_type",
                table: "user_accounts",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "date_of_birth",
                table: "user_infos");

            migrationBuilder.DropColumn(
                name: "user_account_type",
                table: "user_accounts");
        }
    }
}
