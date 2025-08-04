#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;

namespace Pi.BackofficeService.Migrations.Migrations.BackofficeDb
{
    /// <inheritdoc />
    public partial class AddWithdrawError : Migration
    {
        private Guid _revertTransferFail;

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _revertTransferFail = new Guid("ee6606e9-b22d-4464-b7eb-4a684900d798");
            migrationBuilder.InsertData(
                "error_mappings",
                new[] { "id", "machine", "state", "error_type", "description", "suggestion" },
                new object[,]
                {
                    {
                        new Guid("d923aa4d-3561-46bc-bec0-25a8e925905f").ToString(), Machine.Withdraw.ToString(), "FXTransferFailed",
                        ProductType.GlobalEquity.ToString(), "FX Transfer Fail", null
                    },
                    {
                        new Guid("030341d6-31b3-4a98-b496-7782803d4e15").ToString(), Machine.Withdraw.ToString(), "KKPWithdrawalFailed",
                        ProductType.GlobalEquity.ToString(), "Withdraw Fail", null
                    },
                    {
                        _revertTransferFail.ToString(), Machine.Withdraw.ToString(), "RevertTransferFailed",
                        ProductType.GlobalEquity.ToString(), "Withdraw Request Fail", "Manual Re-allocation Required"
                    },
                });

            migrationBuilder.InsertData(
                "error_handler_actions",
                new[] { "id", "error_mapping_id", "action" },
                new object[,]
                {
                    { new Guid("84a61396-c8e7-4383-b0db-d297c1526c70").ToString(), _revertTransferFail.ToString(), Method.CcyAllocationTransfer.ToString() }
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
                    {ProductType.GlobalEquity.ToString(), "FXTransferFailed"},
                    {ProductType.GlobalEquity.ToString(), "KKPWithdrawalFailed"},
                    {ProductType.GlobalEquity.ToString(), "RevertTransferFailed"},
                });

            migrationBuilder.DeleteData(
                "error_handler_actions",
                "error_mapping_id",
                new object[]
                {
                    _revertTransferFail.ToString()
                });
        }
    }
}
