using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.BackofficeService.Migrations.Migrations.DataSeeding
{
    /// <inheritdoc />
    public partial class UpdateDuplicateMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("1395482b-939f-46f5-a039-4bd3bdf3edd8"),
                column: "suggestion",
                value: "Manual Re-allocation Required");

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("a151f28d-7439-411c-9928-6ca26d7ec82f"),
                columns: new[] { "description", "product_type" },
                values: new object[] { "Transfer Request Failed", "GlobalEquity" });

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("f3092460-34af-4c24-9b87-d24df13a2872"),
                column: "description",
                value: "Fail to Deposit");

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("f4432ba1-6019-44e2-ae9c-c3d3ac64a2ab"),
                column: "suggestion",
                value: "Manual Re-allocation Required");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("1395482b-939f-46f5-a039-4bd3bdf3edd8"),
                column: "suggestion",
                value: "Contact Technical Team");

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("a151f28d-7439-411c-9928-6ca26d7ec82f"),
                columns: new[] { "description", "product_type" },
                values: new object[] { "Fail to Deposit Fund", null });

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("f3092460-34af-4c24-9b87-d24df13a2872"),
                column: "description",
                value: "Fail to Deposit Fund");

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("f4432ba1-6019-44e2-ae9c-c3d3ac64a2ab"),
                column: "suggestion",
                value: "Contact Technical Team");
        }
    }
}
