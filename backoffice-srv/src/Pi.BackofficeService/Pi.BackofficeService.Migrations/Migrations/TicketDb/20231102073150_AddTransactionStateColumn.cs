#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Pi.BackofficeService.Migrations.Migrations.TicketDb
{
    /// <inheritdoc />
    public partial class AddTransactionStateColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "transaction_state",
                table: "ticket_state",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "transaction_state",
                table: "ticket_state");
        }
    }
}
