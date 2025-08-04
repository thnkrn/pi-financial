using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.BackofficeService.Migrations.Migrations.DataSeeding
{
    /// <inheritdoc />
    public partial class ModifiedStateNameForWalletV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("f534e848-9441-40bb-8878-1b9b5b3a801f"),
                column: "state",
                value: "UpBackFailedRequireActionSba");

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("f9e00911-c580-48f2-9302-d7e10388507f"),
                column: "state",
                value: "UpBackFailedRequireActionSetTrade");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("f534e848-9441-40bb-8878-1b9b5b3a801f"),
                column: "state",
                value: "UpBackFailedAwaitingActionSba");

            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("f9e00911-c580-48f2-9302-d7e10388507f"),
                column: "state",
                value: "UpBackFailedAwaitingActionSetTrade");
        }
    }
}
