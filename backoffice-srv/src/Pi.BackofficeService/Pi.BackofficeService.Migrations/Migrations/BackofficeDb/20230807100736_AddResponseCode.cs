#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Pi.BackofficeService.Migrations.Migrations.BackofficeDb
{
    /// <inheritdoc />
    public partial class AddResponseCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(name: "error_mappings", newName: "response_codes");
            migrationBuilder.RenameTable(name: "error_handler_actions", newName: "response_code_actions");
            migrationBuilder.RenameColumn(
                table: "response_codes",
                name: "error_type",
                newName: "product_type");
            migrationBuilder.RenameColumn(
                table: "response_code_actions",
                name: "error_mapping_id",
                newName: "response_code_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(name: "response_codes", newName: "error_mappings");
            migrationBuilder.RenameTable(name: "response_code_actions", newName: "error_handler_actions");
            migrationBuilder.RenameColumn(
                table: "error_mappings",
                name: "product_type",
                newName: "error_type");
            migrationBuilder.RenameColumn(
                table: "error_handler_actions",
                name: "response_code_id",
                newName: "error_mapping_id");
        }
    }
}
