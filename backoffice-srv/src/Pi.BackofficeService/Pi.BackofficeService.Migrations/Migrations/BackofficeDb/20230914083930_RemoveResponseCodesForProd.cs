#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Pi.BackofficeService.Migrations.Migrations.BackofficeDb
{
    /// <inheritdoc />
    public partial class RemoveResponseCodesForProd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                "response_codes",
                new[] { "state", "product_type" },
                new object[,]
                {
                    { "Received", "" },
                    { "Failed", "" },
                    { "DepositWaitingForPayment", "" },
                    { "DepositPaymentReceived", "" },
                    { "DepositPaymentSourceValidating", "" },
                    { "DepositPaymentNameValidating", "" },
                    { "DepositQRCodeGenerating", "" },
                    { "TransactionNoGenerating", "" },
                    { "DepositWaitingforPayment", "" },
                    { "DepositCompleted", "" },
                    { "DepositFailed", "" },
                    { "DepositFailedAmountMismatch", "" },
                    { "DepositFailed", "" },
                    { "DepositRefundFailed", "" },
                    { "DepositWaitingForPayment", "" }
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
