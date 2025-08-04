#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;

namespace Pi.BackofficeService.Migrations.Migrations.BackofficeDb
{
    /// <inheritdoc />
    public partial class AddThaiWithdrawResponseCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                "response_codes",
                new[] { "id", "machine", "state", "product_type", "description", "suggestion" },
                new object[,]
                {
                    { new Guid("0cb5682c-1a8c-4b05-a26c-cfaedbf0a91d"), Machine.Withdraw.ToString(), "CashWithdrawWaitingForOtpValidation", ProductType.ThaiEquity.ToString(), "OTP Required", null },
                    { new Guid("f6963e61-323a-41c2-b83d-5344222491dc"), Machine.Withdraw.ToString(), "TransferRequestFailed", ProductType.ThaiEquity.ToString(), "Transfer Request Fail", "Contact Technical Team" },
                    { new Guid("1cf08033-a029-434f-907a-9a7d88787b7f"), Machine.Withdraw.ToString(), "RevertTransferFailed", ProductType.ThaiEquity.ToString(), "Revert Transaction Fail", "Contact Technical Team and check both front and back" },
                    { new Guid("001fb893-7f55-4721-b34d-175ca4cf6a82"), Machine.Withdraw.ToString(), "RevertTransferSuccess", ProductType.ThaiEquity.ToString(), "Revert Transaction Success", null },
                    { new Guid("c6b59d2c-c75c-4f52-8114-97926a61bcf0"), Machine.Withdraw.ToString(), "WithdrawalFailed", ProductType.ThaiEquity.ToString(), "Withdraw Failed - Pending Revert", null },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                "response_codes",
                new[] { "state" },
                new object[,]
                {
                    {"CashWithdrawWaitingForOtpValidation", ProductType.ThaiEquity.ToString()},
                    {"TransferRequestFailed", ProductType.ThaiEquity.ToString()},
                    {"RevertTransferFailed", ProductType.ThaiEquity.ToString()},
                    {"RevertTransferSuccess", ProductType.ThaiEquity.ToString()},
                    {"WithdrawalFailed", ProductType.ThaiEquity.ToString()},
                });
        }
    }
}
