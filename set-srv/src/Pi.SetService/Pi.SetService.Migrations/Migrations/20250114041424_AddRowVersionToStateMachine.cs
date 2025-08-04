using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.SetService.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddRowVersionToStateMachine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "row_version",
                table: "equity_order_state",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "row_version",
                table: "equity_order_state");
        }
    }
}
