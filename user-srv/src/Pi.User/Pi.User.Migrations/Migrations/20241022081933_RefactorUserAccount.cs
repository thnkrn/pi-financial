using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.User.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class RefactorUserAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_accounts_user_infos_user_id1",
                table: "user_accounts");

            migrationBuilder.DropIndex(
                name: "ix_user_accounts_user_id1",
                table: "user_accounts");

            migrationBuilder.DropColumn(
                name: "user_id1",
                table: "user_accounts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "user_id1",
                table: "user_accounts",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "ix_user_accounts_user_id1",
                table: "user_accounts",
                column: "user_id1");

            migrationBuilder.AddForeignKey(
                name: "fk_user_accounts_user_infos_user_id1",
                table: "user_accounts",
                column: "user_id1",
                principalTable: "user_infos",
                principalColumn: "id");
        }
    }
}
