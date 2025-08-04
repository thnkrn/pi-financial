using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pi.BackofficeService.Migrations.Migrations.DataSeeding
{
    /// <inheritdoc />
    public partial class AddNewActionForWalletV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("723f4edf-fb08-42da-9bd9-60cf7eaead9c"),
                column: "product_type",
                value: null);

            migrationBuilder.InsertData(
                table: "response_codes",
                columns: new[] { "id", "description", "is_filterable", "machine", "product_type", "state", "suggestion" },
                values: new object[,]
                {
                    { new Guid("f534e848-9441-40bb-8878-1b9b5b3a801f"), "Trading Account Deposit Fail", true, "Deposit", null, "UpBackFailedAwaitingActionSba", "Check Customer Trading Account Balance, before Manual Allocate" },
                    { new Guid("f9e00911-c580-48f2-9302-d7e10388507f"), "SetTrade Trading Account Deposit Fail", true, "Deposit", null, "UpBackFailedAwaitingActionSetTrade", "Check MT4, Check Customer SetTrade Account Balance, before Manual Allocate" }
                });

            migrationBuilder.InsertData(
                table: "response_code_actions",
                columns: new[] { "id", "action", "response_code_id" },
                values: new object[,]
                {
                    { new Guid("b0c5da2a-0526-41b1-a5ee-bc6ab2b5090a"), "SetTradeAllocationTransfer", new Guid("f9e00911-c580-48f2-9302-d7e10388507f") },
                    { new Guid("bf0c2940-1080-453d-a7ca-6718a880cda2"), "SbaAllocationTransfer", new Guid("f534e848-9441-40bb-8878-1b9b5b3a801f") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("b0c5da2a-0526-41b1-a5ee-bc6ab2b5090a"));

            migrationBuilder.DeleteData(
                table: "response_code_actions",
                keyColumn: "id",
                keyValue: new Guid("bf0c2940-1080-453d-a7ca-6718a880cda2"));

            migrationBuilder.DeleteData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("f534e848-9441-40bb-8878-1b9b5b3a801f"));

            migrationBuilder.DeleteData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("f9e00911-c580-48f2-9302-d7e10388507f"));

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("723f4edf-fb08-42da-9bd9-60cf7eaead9c"),
                column: "product_type",
                value: "ThaiEquity");
        }
    }
}
