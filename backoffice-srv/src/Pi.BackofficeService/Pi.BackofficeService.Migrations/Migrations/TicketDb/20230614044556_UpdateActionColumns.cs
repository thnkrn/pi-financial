#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Pi.BackofficeService.Migrations.Migrations.TicketDb
{
    /// <inheritdoc />
    public partial class UpdateActionColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "method",
                table: "ticket_state",
                newName: "request_action");

            migrationBuilder.AddColumn<string>(
                name: "checker_action",
                table: "ticket_state",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "checker_action",
                table: "ticket_state");

            migrationBuilder.RenameColumn(
                name: "request_action",
                table: "ticket_state",
                newName: "method");
        }
    }
}
