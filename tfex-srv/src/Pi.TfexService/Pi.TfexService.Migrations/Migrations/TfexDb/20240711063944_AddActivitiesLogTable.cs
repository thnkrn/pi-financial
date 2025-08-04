using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.TfexService.Migrations.Migrations.TfexDb
{
    /// <inheritdoc />
    public partial class AddActivitiesLogTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "activities_logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    user_id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    customer_code = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    account_code = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    request_type = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, collation: "utf8mb4_0900_ai_ci"),
                    request_body = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    order_no = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    response_body = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci"),
                    requested_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    completed_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    row_version = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_activities_logs", x => x.id);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "ix_activities_logs_order_no",
                table: "activities_logs",
                column: "order_no");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "activities_logs");
        }
    }
}
