using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.Financial.FundService.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRemoveUniqueIndexFromOrderNumberColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_fund_order_state_order_no",
                table: "fund_order_state");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_fund_order_state_order_no",
                table: "fund_order_state",
                column: "order_no",
                unique: true);
        }
    }
}
