using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.SetService.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewerIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "reviewer_id",
                table: "sbl_orders",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "reviewer_id",
                table: "sbl_orders");
        }
    }
}
