#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;

namespace Pi.BackofficeService.Migrations.Migrations.BackofficeDb
{
    /// <inheritdoc />
    public partial class PopulateResponseCodesForFinalState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                "response_codes",
                new[] { "id", "machine", "state", "product_type", "description", "suggestion" },
                new object[,]
                {
                    { new Guid("f3884d07-5025-4c8f-8ad0-8900e6dec7af"), Machine.Deposit.ToString(), "Final", ProductType.GlobalEquity.ToString(), "Deposit Completed", null },
                    { new Guid("0ce1ee39-b53c-4c27-b111-31c24ab2d88a"), Machine.Withdraw.ToString(), "Final", null, "Withdraw Completed", null },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                "response_codes",
                new[] { "product_type", "machine", "state" },
                new object[,]
                {
                    {ProductType.GlobalEquity.ToString(), Machine.Deposit.ToString(), "Final"},
                    {null, Machine.Withdraw.ToString(), "Final"},
                });
        }
    }
}
