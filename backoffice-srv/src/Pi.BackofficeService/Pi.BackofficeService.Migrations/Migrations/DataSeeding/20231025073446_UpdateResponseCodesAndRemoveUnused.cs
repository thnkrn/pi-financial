using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.BackofficeService.Migrations.Migrations.DataSeeding
{
    /// <inheritdoc />
    public partial class UpdateResponseCodesAndRemoveUnused : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("c1a73f39-b127-427b-806c-206952194ff4"));

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("1395482b-939f-46f5-a039-4bd3bdf3edd8"),
                column: "description",
                value: "Manual Allocation in XNT Failed");

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("f2cc09c8-a739-4797-b00a-492e503f7c8d"),
                column: "description",
                value: "Unfavorable FX (rate over)");

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("f4432ba1-6019-44e2-ae9c-c3d3ac64a2ab"),
                column: "description",
                value: "Manual Allocation in XNT Failed");

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("fec7cb30-7a63-4248-a7c0-c6d2f9c0cf1a"),
                column: "description",
                value: "Deposit Completed");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("1395482b-939f-46f5-a039-4bd3bdf3edd8"),
                column: "description",
                value: "CCY Allocation Transfer Fail");

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("f2cc09c8-a739-4797-b00a-492e503f7c8d"),
                column: "description",
                value: "Unfavourable FX (rate over)");

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("f4432ba1-6019-44e2-ae9c-c3d3ac64a2ab"),
                column: "description",
                value: "CCY Allocation Transfer Fail");

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("fec7cb30-7a63-4248-a7c0-c6d2f9c0cf1a"),
                column: "description",
                value: "Trading Account Deposit Completed");

            migrationBuilder.InsertData(
                table: "response_codes",
                columns: new[] { "id", "description", "machine", "product_type", "state", "suggestion" },
                values: new object[] { new Guid("c1a73f39-b127-427b-806c-206952194ff4"), "Withdraw Failed - Pending Revert", "Withdraw", "ThaiEquity", "WithdrawalFailed", null });
        }
    }
}
