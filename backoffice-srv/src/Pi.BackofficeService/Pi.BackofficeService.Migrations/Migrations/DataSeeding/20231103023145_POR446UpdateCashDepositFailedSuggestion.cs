using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.BackofficeService.Migrations.Migrations.DataSeeding
{
    /// <inheritdoc />
    public partial class POR446UpdateCashDepositFailedSuggestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("723f4edf-fb08-42da-9bd9-60cf7eaead9c"),
                column: "suggestion",
                value: "Contact Technical Team and check Customer Trading Account Balance");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "response_codes",
                keyColumn: "id",
                keyValue: new Guid("723f4edf-fb08-42da-9bd9-60cf7eaead9c"),
                column: "suggestion",
                value: "Contact Technical Support and manual update Fund to SBA");
        }
    }
}
