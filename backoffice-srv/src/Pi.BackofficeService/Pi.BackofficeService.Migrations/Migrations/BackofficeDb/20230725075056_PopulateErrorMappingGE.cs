#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;

namespace Pi.BackofficeService.Migrations.Migrations.BackofficeDb;

/// <inheritdoc />
public partial class PopulateErrorMappingGE : Migration
{
    private Guid _nameMismatch;
    private Guid _fXTransferFailed;
    private Guid _fXTransferInsufficientBalance;
    private Guid _fxFailed;
    private Guid _fxRateCompareFailed;

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _nameMismatch = new Guid("76ca8656-4dfb-4cd8-9545-8659390eaadf");
        _fXTransferFailed = new Guid("6c2dec69-d5f0-4417-8632-b5313e59a37d");
        _fXTransferInsufficientBalance = new Guid("b6c9f8ff-020a-4dde-8fda-538ea923ee7d");
        _fxFailed = new Guid("6fcb096a-de64-44d7-8964-dae8767269e0");
        _fxRateCompareFailed = new Guid("9bed58a9-5ab1-4164-86a6-46d023d25f71");
        migrationBuilder.InsertData(
            "error_mappings",
            new[] { "id", "machine", "state", "error_type", "description", "suggestion" },
            new object[,]
            {
                {
                    _nameMismatch.ToString(), Machine.Deposit.ToString(), "DepositFailedNameMismatch",
                    ProductType.GlobalEquity.ToString(), "Name Mismatch", "Refund Required"
                },
                {
                    _fXTransferFailed.ToString(), Machine.Deposit.ToString(), "FXTransferFailed",
                    ProductType.GlobalEquity.ToString(), "FX Transfer Fail", "Manual Allocation Required"
                },
                {
                    _fXTransferInsufficientBalance.ToString(), Machine.Deposit.ToString(), "FXTransferInsufficientBalance",
                    ProductType.GlobalEquity.ToString(), "Insufficient Fund in Master Account", "Top up balance is required"
                },
                {
                    _fxFailed.ToString(), Machine.Deposit.ToString(), "FXFailed",
                    ProductType.GlobalEquity.ToString(), "Fail to convert FX", "Full Refund"
                },
                {
                    _fxRateCompareFailed.ToString(), Machine.Deposit.ToString(), "FXRateCompareFailed",
                    ProductType.GlobalEquity.ToString(), "Unfavourable FX", "Full Refund"
                }
            });

        migrationBuilder.InsertData(
            "error_handler_actions",
            new[] { "id", "error_mapping_id", "action" },
            new object[,]
            {
                { new Guid("9db3ec80-2084-499f-995a-e52c2ab90ea5").ToString(), _nameMismatch.ToString(), Method.Refund.ToString() },
                { new Guid("2a78e677-8350-42ba-87ae-8d970f7059e7").ToString(), _fXTransferFailed.ToString(), Method.CcyAllocationTransfer.ToString() },
                { new Guid("420ccdc5-dd7d-4c0c-ada9-c8cc1714313d").ToString(), _fxFailed.ToString(), Method.Refund.ToString() },
                { new Guid("1629f0f3-6d9e-4234-a9d0-3dc8d446d5f4").ToString(), _fxRateCompareFailed.ToString(), Method.Refund.ToString() },
                { new Guid("b14cbe01-49ce-4555-93fd-2bd2bcdedfbc").ToString(), _fXTransferInsufficientBalance.ToString(), Method.CcyAllocationTransfer.ToString() }
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            "error_mappings",
            new[] { "error_type", "state" },
            new object[,]
            {
                {ProductType.GlobalEquity.ToString(), "DepositFailedNameMismatch"},
                {ProductType.GlobalEquity.ToString(), "FXTransferFailed"},
                {ProductType.GlobalEquity.ToString(), "FXTransferInsufficientBalance"},
                {ProductType.GlobalEquity.ToString(), "FXFailed"},
                {ProductType.GlobalEquity.ToString(), "FXRateCompareFailed"},
            });

        migrationBuilder.DeleteData(
            "error_handler_actions",
            "error_mapping_id",
            new object[]
            {
                _nameMismatch.ToString(),
                _fXTransferFailed.ToString(),
                _fxFailed.ToString(),
                _fxRateCompareFailed.ToString(),
                _fXTransferInsufficientBalance.ToString()
            });
    }
}
