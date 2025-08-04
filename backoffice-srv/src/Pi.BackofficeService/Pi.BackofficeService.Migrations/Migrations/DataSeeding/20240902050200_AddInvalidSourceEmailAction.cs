using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pi.BackofficeService.Migrations.Migrations.DataSeeding
{
    /// <inheritdoc />
    public partial class AddInvalidSourceEmailAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "response_codes",
                columns: new[] { "id", "description", "is_filterable", "machine", "product_type", "state", "suggestion" },
                values: new object[,]
                {
                    { new Guid("8246f2c5-e8dc-4876-956a-9c9fb2361610"), "Incorrect Source - Email not success", true, "Deposit", null, "InvalidSourceSendEmailFailed", "Manually email to customer for documents before proceed next step" },
                    { new Guid("fc8220ca-5728-468f-85f8-406e1c2f0ff4"), "Incorrect Source - Email success", true, "Deposit", null, "InvalidSourceSendEmailSuccess", "Waiting customer document before proceed next step" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("8246f2c5-e8dc-4876-956a-9c9fb2361610"));

            migrationBuilder.DeleteData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("fc8220ca-5728-468f-85f8-406e1c2f0ff4"));
        }
    }
}
