#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;

namespace Pi.BackofficeService.Migrations.Migrations.BackofficeDb;

/// <inheritdoc />
public partial class AddPopulateErrorCode : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        var nameMismatch = new Guid("2c6b0daa-221a-4766-9ef8-eab63f23280a");
        migrationBuilder.InsertData(
            "error_mappings",
            new[] { "id", "machine", "state", "description", "suggestion" },
            new object[,]
            {
                {
                    new Guid("70772bb6-b7c7-4f66-8573-2217ec88aa26").ToString(), Machine.Deposit.ToString(), "DepositGenerateQRCodeFailed",
                    "Fail to Generate QR", "Contact Technical Team"
                },
                {
                    nameMismatch.ToString(), Machine.Deposit.ToString(), "DepositFailedNameMismatch",
                    "Name Mismatch", "Investigate Name"
                },
                {
                    new Guid("f789328c-7859-44d9-8755-ad9de22c3774").ToString(), Machine.Deposit.ToString(), "DepositFailedInvalidSource",
                    "Invalid Source", "Investigate Source of Fund"
                },
                {
                    new Guid("94615811-5391-4655-9112-61e93f80a98b").ToString(), Machine.Deposit.ToString(), "NotApprovedFrontOffice",
                    "Can not Approve to Front Office", "Contact Technical Team"
                },
                {
                    new Guid("68bedff6-bb54-46ca-b751-efa015c1d8d5").ToString(), Machine.Deposit.ToString(), "LockTableBackOffice",
                    "Lock table in Back Office", "Contact Technical Team"
                },
                {
                    new Guid("9112a77a-0c73-4798-898e-5aa2badac09a").ToString(), Machine.Deposit.ToString(), "ConnectionTimeOut", "Connection timed out",
                    "Contact Technical Team"
                },
                {
                    new Guid("f1199c64-f629-4134-80dd-d5b463bc9e19").ToString(), Machine.Deposit.ToString(), "InternalServerError",
                    "Internal server error", "Contact Technical Team"
                },
                {
                    new Guid("b78ea701-410d-4530-a60b-623d301a31d6").ToString(), Machine.Deposit.ToString(), "DepositFailed", "Unexpected error occurred",
                    "Contact Technical Team"
                },
                {
                    new Guid("1109d446-37ac-44f1-8783-cc2b25ddafe4").ToString(), Machine.Deposit.ToString(), "DepositFailedAmountMismatch", "Amount Mismatch",
                    "Investigate Amount"
                },
                {
                    new Guid("912ae161-26f0-4c98-8965-f48429658d01").ToString(), Machine.Deposit.ToString(), "DepositRefundFailed", "Unable to Refund",
                    "Contact Technical Team"
                }
            });

        migrationBuilder.InsertData(
            "error_handler_actions",
            new[] { "id", "error_mapping_id", "action" },
            new object[,]
            {
                { new Guid("b06af7d6-39be-4668-b2ed-b3f12935dd6d").ToString(), nameMismatch.ToString(), Method.Approve.ToString()},
                { new Guid("2a5f197f-b036-46d6-8c7a-d08aed5b5164").ToString(), nameMismatch.ToString(), Method.Refund.ToString()},
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            "error_mappings",
            "state",
            new object[]
            {
                "DepositGenerateQRCodeFailed",
                "DepositFailedNameMismatched",
                "DepositFailedInvalidSource",
                "NotApprovedFrontOffice",
                "LockTableBackOffice",
                "ConnectionTimeOut",
                "InternalServerError",
                "DepositFailed",
                "DepositFailedAmountMismatch",
                "DepositRefundFailed"
            });

        migrationBuilder.DeleteData(
            "error_handler_actions",
            "action",
            new object[]
            {
                Method.Approve.ToString(),
                Method.Refund.ToString(),
            });
    }
}
