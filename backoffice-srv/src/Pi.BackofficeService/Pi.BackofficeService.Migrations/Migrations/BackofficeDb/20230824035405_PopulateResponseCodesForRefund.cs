#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;

namespace Pi.BackofficeService.Migrations.Migrations.BackofficeDb
{
    /// <inheritdoc />
    public partial class PopulateResponseCodesForRefund : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                "response_codes",
                new[] { "id", "machine", "state", "product_type", "description", "suggestion" },
                new object[,]
                {
                    { new Guid("e6cf7119-98fe-4764-bc01-ccb1096a1462"), Machine.Deposit.ToString(), "DepositRefundSucceed", null, "Refund Success", null },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                "response_codes",
                new[] { "product_type", "state" },
                new object[,]
                {
                    {null, "DepositRefundSucceed"},
                });
        }
    }
}
