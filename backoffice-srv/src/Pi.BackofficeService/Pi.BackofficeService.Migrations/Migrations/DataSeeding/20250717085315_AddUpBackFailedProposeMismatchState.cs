using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pi.BackofficeService.Migrations.Migrations.DataSeeding
{
    /// <inheritdoc />
    public partial class AddUpBackFailedProposeMismatchState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "response_codes",
                columns: new[] { "id", "description", "is_filterable", "machine", "product_type", "state", "suggestion" },
                values: new object[] { new Guid("e7267f12-7e3d-4181-8bf2-ef87e8903842"), "Pending for manual in SBA", true, "Deposit", null, "UpBackFailedPurposeMismatch", "manual in SBA" });

            migrationBuilder.InsertData(
                table: "response_code_actions",
                columns: new[] { "id", "action", "response_code_id" },
                values: new object[,]
                {
                    { new Guid("1400e565-e420-40ed-88b7-2f300590e14c"), "ChangeStatusToFail", new Guid("e7267f12-7e3d-4181-8bf2-ef87e8903842") },
                    { new Guid("8ff5de2a-d090-4648-8482-dd096f104259"), "ChangeStatusToSuccess", new Guid("e7267f12-7e3d-4181-8bf2-ef87e8903842") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("1400e565-e420-40ed-88b7-2f300590e14c"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("8ff5de2a-d090-4648-8482-dd096f104259"));

            migrationBuilder.DeleteData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("e7267f12-7e3d-4181-8bf2-ef87e8903842"));
        }
    }
}
