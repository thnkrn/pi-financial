using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.BackofficeService.Migrations.Migrations.DataSeeding
{
    /// <inheritdoc />
    public partial class AUDI261UpdateResponseCodeAndActionForSBAManualTransfer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "response_code_actions",
                columns: new[] { "id", "action", "response_code_id" },
                values: new object[] { new Guid("9462554b-f394-4572-b29f-0e5830b03889"), "SbaAllocationTransfer", new Guid("723f4edf-fb08-42da-9bd9-60cf7eaead9c") });

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("723f4edf-fb08-42da-9bd9-60cf7eaead9c"),
                column: "suggestion",
                value: "Check Customer Trading Account Balance, before Manual Allocate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("9462554b-f394-4572-b29f-0e5830b03889"));

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("723f4edf-fb08-42da-9bd9-60cf7eaead9c"),
                column: "suggestion",
                value: "Contact Technical Team and check Customer Trading Account Balance");
        }
    }
}
