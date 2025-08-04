using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.TfexService.Migrations.Migrations.TfexDb
{
    /// <inheritdoc />
    public partial class AddColumnForActivityLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "failed_reason",
                table: "activities_logs",
                type: "longtext",
                nullable: true,
                collation: "utf8mb4_0900_ai_ci");

            migrationBuilder.AddColumn<bool>(
                name: "is_success",
                table: "activities_logs",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "price",
                table: "activities_logs",
                type: "longtext",
                nullable: true,
                collation: "utf8mb4_0900_ai_ci");

            migrationBuilder.AddColumn<string>(
                name: "price_type",
                table: "activities_logs",
                type: "longtext",
                nullable: true,
                collation: "utf8mb4_0900_ai_ci");

            migrationBuilder.AddColumn<string>(
                name: "qty",
                table: "activities_logs",
                type: "longtext",
                nullable: true,
                collation: "utf8mb4_0900_ai_ci");

            migrationBuilder.AddColumn<string>(
                name: "reject_code",
                table: "activities_logs",
                type: "longtext",
                nullable: true,
                collation: "utf8mb4_0900_ai_ci");

            migrationBuilder.AddColumn<string>(
                name: "reject_reason",
                table: "activities_logs",
                type: "longtext",
                nullable: true,
                collation: "utf8mb4_0900_ai_ci");

            migrationBuilder.AddColumn<string>(
                name: "side",
                table: "activities_logs",
                type: "longtext",
                nullable: true,
                collation: "utf8mb4_0900_ai_ci");

            migrationBuilder.AddColumn<string>(
                name: "symbol",
                table: "activities_logs",
                type: "longtext",
                nullable: true,
                collation: "utf8mb4_0900_ai_ci");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "failed_reason",
                table: "activities_logs");

            migrationBuilder.DropColumn(
                name: "is_success",
                table: "activities_logs");

            migrationBuilder.DropColumn(
                name: "price",
                table: "activities_logs");

            migrationBuilder.DropColumn(
                name: "price_type",
                table: "activities_logs");

            migrationBuilder.DropColumn(
                name: "qty",
                table: "activities_logs");

            migrationBuilder.DropColumn(
                name: "reject_code",
                table: "activities_logs");

            migrationBuilder.DropColumn(
                name: "reject_reason",
                table: "activities_logs");

            migrationBuilder.DropColumn(
                name: "side",
                table: "activities_logs");

            migrationBuilder.DropColumn(
                name: "symbol",
                table: "activities_logs");
        }
    }
}
