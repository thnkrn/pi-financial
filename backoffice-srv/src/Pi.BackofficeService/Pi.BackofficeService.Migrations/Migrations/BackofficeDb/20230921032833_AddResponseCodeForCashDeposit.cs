#nullable disable

#region

using Microsoft.EntityFrameworkCore.Migrations;
using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;

#endregion

namespace Pi.BackofficeService.Migrations.Migrations.BackofficeDb
{
    /// <inheritdoc />
    public partial class AddResponseCodeForCashDeposit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                "response_codes",
                new[] { "id", "machine", "state", "product_type", "description", "suggestion" },
                new object[,]
                {
                    { new Guid("fd878e60-0776-4c84-a288-08f419a1cb61"), Machine.Deposit.ToString(), "CashDepositTradingPlatformUpdating", ProductType.ThaiEquity.ToString(), "Updating Back", null },
                    { new Guid("9cc00ad4-5dfb-4873-8407-0e630c137044"), Machine.Deposit.ToString(), "CashDepositWaitingForGateway", ProductType.ThaiEquity.ToString(), "Waiting for Response from Back", null },
                    { new Guid("9ad97a4c-d06f-46dd-b8fe-8e9d9eb4f0c9"), Machine.Deposit.ToString(), "CashDepositWaitingForTradingPlatform", ProductType.ThaiEquity.ToString(), "Updating Settrade TFEX ", null },
                    { new Guid("fb8d318e-ffd4-44e4-b3a9-974c279ff75b"), Machine.Deposit.ToString(), "CashDepositCompleted", ProductType.ThaiEquity.ToString(), "Trading Account Deposit Completed", null },
                    { new Guid("9c11fc8d-e3fa-4815-bd34-79348969eb18"), Machine.Deposit.ToString(), "CashDepositFailed", ProductType.ThaiEquity.ToString(), "Trading Account Deposit Fail", "Contact Technical Support" },
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
                    { "CashDepositTradingPlatformUpdating", ProductType.ThaiEquity.ToString() },
                    { "CashDepositWaitingForGateway", ProductType.ThaiEquity.ToString() },
                    { "CashDepositWaitingForTradingPlatform", ProductType.ThaiEquity.ToString() },
                    { "CashDepositCompleted", ProductType.ThaiEquity.ToString() },
                    { "CashDepositFailed", ProductType.ThaiEquity.ToString() },
                });
        }
    }
}
