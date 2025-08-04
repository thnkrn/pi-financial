using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.SetService.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSblOrderUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "sbl_order_no",
                table: "sbl_orders");

            migrationBuilder.DropColumn(
                name: "order_no",
                table: "sbl_orders");

            migrationBuilder.AddColumn<ulong>(
                name: "order_id",
                table: "sbl_orders",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<DateOnly>(
                name: "created_at_date",
                table: "sbl_orders",
                type: "date",
                nullable: false,
                computedColumnSql: "DATE(created_at)");

            migrationBuilder.CreateIndex(
                name: "unique_order_date",
                table: "sbl_orders",
                columns: new[] { "order_id", "created_at_date" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "unique_order_date",
                table: "sbl_orders");

            migrationBuilder.DropColumn(
                name: "created_at_date",
                table: "sbl_orders");

            migrationBuilder.DropColumn(
                name: "order_id",
                table: "sbl_orders");

            migrationBuilder.AddColumn<string>(
                name: "order_no",
                table: "sbl_orders",
                type: "varchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "sbl_order_no",
                table: "sbl_orders",
                column: "order_no",
                unique: true);
        }
    }
}
