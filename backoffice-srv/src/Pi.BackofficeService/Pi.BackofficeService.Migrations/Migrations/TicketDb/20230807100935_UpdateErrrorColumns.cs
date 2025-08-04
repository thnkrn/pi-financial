#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Pi.BackofficeService.Migrations.Migrations.TicketDb
{
    /// <inheritdoc />
    public partial class UpdateErrrorColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "error_mapping_id",
                table: "ticket_state",
                newName: "response_code_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "response_code_id",
                table: "ticket_state",
                newName: "error_mapping_id");
        }
    }
}
