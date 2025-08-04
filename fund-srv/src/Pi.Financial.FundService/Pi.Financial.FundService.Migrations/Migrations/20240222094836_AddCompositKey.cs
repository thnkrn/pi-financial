using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pi.Financial.FundService.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddCompositKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_fund_order_state_order_no_order_type",
                table: "fund_order_state",
                columns: new[] { "order_no", "order_type" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_fund_order_state_order_no_order_type",
                table: "fund_order_state");
        }
    }
}
