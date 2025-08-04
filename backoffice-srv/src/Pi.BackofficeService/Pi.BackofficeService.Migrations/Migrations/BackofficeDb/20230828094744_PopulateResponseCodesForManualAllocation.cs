#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;

namespace Pi.BackofficeService.Migrations.Migrations.BackofficeDb
{
    /// <inheritdoc />
    public partial class PopulateResponseCodesForManualAllocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                "response_codes",
                new[] { "id", "machine", "state", "product_type", "description", "suggestion" },
                new object[,]
                {
                    { new Guid("816538ea-faed-4651-8963-0097b1ec994d"), Machine.Deposit.ToString(), "ManualAllocationFailed", ProductType.GlobalEquity.ToString(), "CCY Allocation Transfer Failed", "Contact Technical Team" },
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
                    {ProductType.GlobalEquity.ToString(), "ManualAllocationFailed"},
                });
        }
    }
}
