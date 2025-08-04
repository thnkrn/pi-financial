#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;

namespace Pi.BackofficeService.Migrations.Migrations.BackofficeDb
{
    /// <inheritdoc />
    public partial class AddResponseCodeForProd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                "response_codes",
                new[] { "id", "machine", "state", "product_type", "description", "suggestion" },
                new object[,]
                {
                    { new Guid("b6cb7883-3e90-4803-ac5a-91ad4b80eefb"), Machine.Deposit.ToString(), "DepositWaitingForPayment", null, "Waiting for Fund", "Inquire Payment Status" },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                "response_codes",
                new[] { "state", "product_type" },
                new object[,]
                {
                    { "DepositWaitingForPayment", null },
                });
        }
    }
}
