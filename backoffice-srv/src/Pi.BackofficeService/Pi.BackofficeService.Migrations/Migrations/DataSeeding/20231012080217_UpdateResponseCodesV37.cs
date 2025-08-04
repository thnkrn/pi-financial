using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.BackofficeService.Migrations.Migrations.DataSeeding
{
    /// <inheritdoc />
    public partial class UpdateResponseCodesV37 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("0e1158b2-569d-4916-a68c-508c6813cb79"),
                columns: new[] { "description", "suggestion" },
                values: new object[] { "Manual allocation in XNT", "Manual Allocation Required due to failed allocation" });

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("15a9160b-a5eb-4754-98b2-3cfea4c4e0d2"),
                columns: new[] { "description", "suggestion" },
                values: new object[] { "Insufficient Balance", "Alert Finance Team on fund top up" });

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("6e865244-d493-4635-b0f6-7b9b6717d20b"),
                column: "description",
                value: "Incorrect Source");

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("76c845bf-fb3a-490f-928c-54811f0a8739"),
                column: "description",
                value: "Unable to FX");

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("f2cc09c8-a739-4797-b00a-492e503f7c8d"),
                column: "description",
                value: "Unfavourable FX (rate over)");

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("f3092460-34af-4c24-9b87-d24df13a2872"),
                column: "description",
                value: "Fail to Deposit Fund");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("0e1158b2-569d-4916-a68c-508c6813cb79"),
                columns: new[] { "description", "suggestion" },
                values: new object[] { "FX Transfer Fail", "Manual Allocation Required" });

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("15a9160b-a5eb-4754-98b2-3cfea4c4e0d2"),
                columns: new[] { "description", "suggestion" },
                values: new object[] { "Insufficient Fund @ Master Account", "Top up balance is required" });

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("6e865244-d493-4635-b0f6-7b9b6717d20b"),
                column: "description",
                value: "Invalid Source");

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("76c845bf-fb3a-490f-928c-54811f0a8739"),
                column: "description",
                value: "FX Failed");

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("f2cc09c8-a739-4797-b00a-492e503f7c8d"),
                column: "description",
                value: "FX Rate Compare Fail");

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("f3092460-34af-4c24-9b87-d24df13a2872"),
                column: "description",
                value: "Fail to Deposit");
        }
    }
}
