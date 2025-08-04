#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;

namespace Pi.BackofficeService.Migrations.Migrations.BackofficeDb
{
    /// <inheritdoc />
    public partial class PopulateResponseCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                "response_codes",
                new[] { "id", "machine", "state", "product_type", "description", "suggestion" },
                new object[,]
                {
                    // { new Guid(""), Machine.Deposit.ToString(), "Received", null, "Transaction Initiation", null },
                    // { new Guid(""), Machine.Deposit.ToString(), "TransactionNoGenerating", null, "Transaction Initiation", null },
                    // { new Guid(""), Machine.Deposit.ToString(), "DepositQRCodeGenerating", null, "QR Generation", null },
                    // { new Guid(""), Machine.Deposit.ToString(), "DepositPaymentReceived", null, "Fund Received", null },
                    // { new Guid(""), Machine.Deposit.ToString(), "DepositPaymentSourceValidating", null, "Transaction Validation", null },
                    // { new Guid(""), Machine.Deposit.ToString(), "DepositPaymentNameValidating", null, "Transaction Name Check", null },
                    { new Guid("91cc38c2-9aba-4c91-962f-f557cd192255"), Machine.Deposit.ToString(), "DepositCompleted", null, "Deposit Completed", null },
                    { new Guid("de72a4c3-7226-4788-8d67-bff2c60e6151"), Machine.Deposit.ToString(), "DepositWaitingForPayment", null, "Waiting for Fund", "Inquire Payment Status" },
                    { new Guid("1f29dad6-2166-4c45-8c19-6c2f33e41db9"), Machine.Deposit.ToString(), "TransferRequestFailed", ProductType.GlobalEquity.ToString(), "Unable To Process Global Transfer Payment", null },
                    { new Guid("dd8ca1cd-22d6-4f6c-8d8d-098be62f1f39"), Machine.Withdraw.ToString(), "TransferRequestFailed", ProductType.GlobalEquity.ToString(), "Transfer Request Fail",  "Contact Technical Team" },
                    { new Guid("0c5cb836-b1b5-4ce7-be30-69914e87cf33"), Machine.Withdraw.ToString(), "WithdrawalFailed", ProductType.GlobalEquity.ToString(), "Withdraw Fail", null },
                    // { new Guid(""), Machine.Withdraw.ToString(), "RevertTransferFailed", ProductType.GlobalEquity.ToString(), "Failover - Withdraw Fail", "Manual Re-allocation Required" }
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
                    { "Received", null },
                    { "TransactionNoGenerating", null },
                    { "DepositQRCodeGenerating", null },
                    { "DepositWaitingForPayment", null },
                    { "DepositPaymentReceived", null },
                    { "DepositPaymentSourceValidating", null },
                    { "DepositPaymentNameValidating", null },
                    { "DepositCompleted", null },
                    { "TransferRequestFailed", ProductType.GlobalEquity.ToString() },
                    {  "TransferRequestFailed", ProductType.GlobalEquity.ToString() },
                    {  "WithdrawalFailed", ProductType.GlobalEquity.ToString() },
                });
        }
    }
}
