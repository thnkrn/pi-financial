#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Pi.BackofficeService.Migrations.Migrations.TicketDb
{
    /// <inheritdoc />
    public partial class UpdateTicketColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "ticket_state",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "method",
                table: "ticket_state",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "customer_name",
                table: "ticket_state",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "error_mapping_id",
                table: "ticket_state",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "request_id",
                table: "ticket_state",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<string>(
                name: "response_address",
                table: "ticket_state",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "transaction_no",
                table: "ticket_state",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "customer_name",
                table: "ticket_state");

            migrationBuilder.DropColumn(
                name: "error_mapping_id",
                table: "ticket_state");

            migrationBuilder.DropColumn(
                name: "request_id",
                table: "ticket_state");

            migrationBuilder.DropColumn(
                name: "response_address",
                table: "ticket_state");

            migrationBuilder.DropColumn(
                name: "transaction_no",
                table: "ticket_state");

            migrationBuilder.UpdateData(
                table: "ticket_state",
                keyColumn: "status",
                keyValue: null,
                column: "status",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "ticket_state",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "ticket_state",
                keyColumn: "method",
                keyValue: null,
                column: "method",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "method",
                table: "ticket_state",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
