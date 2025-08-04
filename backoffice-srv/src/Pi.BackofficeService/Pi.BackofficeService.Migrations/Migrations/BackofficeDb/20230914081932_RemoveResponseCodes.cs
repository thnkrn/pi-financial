#nullable disable

#region

using Microsoft.EntityFrameworkCore.Migrations;
using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;

#endregion

namespace Pi.BackofficeService.Migrations.Migrations.BackofficeDb
{
    /// <inheritdoc />
    public partial class RemoveResponseCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                "response_codes",
                new[] { "state", "product_type" },
                new object[,]
                {
                    { "Received", null },
                    { "Failed", null },
                    { "DepositPaymentReceived", null },
                    { "DepositPaymentSourceValidating", null },
                    { "DepositPaymentNameValidating", null },
                    { "DepositQRCodeGenerating", null },
                    { "TransactionNoGenerating", null },
                    { "DepositFailed", null },
                    { "DepositFailedAmountMismatch", null },
                    { "DepositFailed", null },
                    { "DepositRefundFailed", null },
                    { "DepositWaitingForPayment", null }
                });
            migrationBuilder.InsertData(
                "response_codes",
                new[] { "id", "machine", "state", "product_type", "description", "suggestion" },
                new object[,]
                {
                    { new Guid("aecd798f-6085-4156-89a1-2e11bb36d3a7"), Machine.Deposit.ToString(), "DepositFailed", null, "Unexpected error occurred", "Contact Technical Team" },
                    { new Guid("c2abca1d-78ac-4704-a9e3-1d8ce1df5feb"), Machine.Deposit.ToString(), "DepositFailedAmountMismatch", null, "Amount Mismatch", "Investigate Amount" },
                    { new Guid("5386c145-ac2a-4f96-b46f-ebba27ed23ac"), Machine.Deposit.ToString(), "DepositRefundFailed", null, "Unable to Refund", "Contact Technical Team" },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                "response_codes",
                new[] { "id", "machine", "state", "product_type", "description", "suggestion" },
                new object[,]
                {
                    { new Guid("510af766-94dd-463a-892a-a02e90165aaf"), Machine.Deposit.ToString(), "Received", null, "Transaction Initiation", null },
                    { new Guid("27915f71-1de3-499a-ad34-89deeea852ee"), Machine.Deposit.ToString(), "TransactionNoGenerating", null, "Transaction Initiation", null },
                    { new Guid("8c73323f-138c-428f-ba7c-98c6a46f3878"), Machine.Deposit.ToString(), "DepositQRCodeGenerating", null, "QR Generation", null },
                    { new Guid("b6edf2f1-973e-4eba-b71d-571444209c36"), Machine.Deposit.ToString(), "DepositPaymentReceived", null, "Fund Received", null },
                    { new Guid("807efd48-28ca-4109-a415-181d6471e94b"), Machine.Deposit.ToString(), "DepositPaymentSourceValidating", null, "Transaction Validation", null },
                    { new Guid("424aa908-67fd-482e-9fed-2fc74b24779c"), Machine.Deposit.ToString(), "DepositPaymentNameValidating", null, "Transaction Name Check", null },
                    { new Guid("096a433b-973b-4f30-83b0-c1fb1f7ce035"), Machine.Withdraw.ToString(), "RevertTransferFailed", ProductType.GlobalEquity.ToString(), "Failover - Withdraw Fail", "Manual Re-allocation Required" }
                });
            migrationBuilder.DeleteData(
                "response_codes",
                new[] { "state", "product_type" },
                new object[,]
                {
                    { "DepositFailed", null },
                    { "DepositFailedAmountMismatch", null },
                    { "DepositRefundFailed", null },
                });
        }
    }
}
