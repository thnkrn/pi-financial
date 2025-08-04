#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pi.BackofficeService.Migrations.Migrations.DataSeeding
{
    /// <inheritdoc />
    public partial class AddResponseCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("truncate table response_codes;");
            migrationBuilder.Sql("truncate table response_code_actions;");
            migrationBuilder.InsertData(
                table: "response_codes",
                columns: new[] { "id", "description", "machine", "product_type", "state", "suggestion" },
                values: new object[,]
                {
                    { new Guid("0ac1e94d-990d-4d3c-9fac-8ae42e39351e"), "Waiting for Fund", "Deposit", null, "DepositWaitingForPayment", "Inquire Payment Status" },
                    { new Guid("0e1158b2-569d-4916-a68c-508c6813cb79"), "FX Transfer Fail", "Deposit", "GlobalEquity", "FXTransferFailed", "Manual Allocation Required" },
                    { new Guid("1395482b-939f-46f5-a039-4bd3bdf3edd8"), "CCY Allocation Transfer Fail", "Deposit", "GlobalEquity", "ManualAllocationFailed", "Contact Technical Team" },
                    { new Guid("15a9160b-a5eb-4754-98b2-3cfea4c4e0d2"), "Insufficient Fund @ Master Account", "Deposit", "GlobalEquity", "FXTransferInsufficientBalance", "Top up balance is required" },
                    { new Guid("2203f732-3fbe-4738-95d2-1c0f70603914"), "Refund Success", "Deposit", null, "RefundSucceed", null },
                    { new Guid("220ed567-701a-4903-a8fe-ad5d3cfc43c1"), "Refund Fail", "Deposit", null, "RefundFailed", "Contact Technical Support and Manual Refund Required" },
                    { new Guid("222d19bd-92b9-4c40-bcea-3b404a14146a"), "Amount Mismatch", "Deposit", null, "DepositFailedAmountMismatch", "Refund Required" },
                    { new Guid("2258bbbc-2dbf-4519-9d40-3bfa7e4b6609"), "Revert Transfer Fail", "Withdraw", "ThaiEquity", "RevertTransferFailed", "Contact Technical Team and check Customer Trading Account Balance" },
                    { new Guid("23f0b465-57dc-4b07-be8d-9db340bb5cc0"), "OTP Required", "Withdraw", "ThaiEquity", "CashWithdrawWaitingForOtpValidation", "Waiting for Customer OTP" },
                    { new Guid("3070a7c3-5ef4-4898-b0c2-92efd83f8e9d"), "Waiting for SBA Callback", "Deposit", "ThaiEquity", "CashDepositWaitingForGateway", null },
                    { new Guid("5afa5a4e-d054-4377-a9f4-e808c1c3706f"), "Revert Transfer Success", "Withdraw", "ThaiEquity", "RevertTransferSuccess", null },
                    { new Guid("60245f07-190e-4c94-b2db-bda11e4f8fa1"), "Updating SBA", "Deposit", "ThaiEquity", "CashDepositTradingPlatformUpdating", null },
                    { new Guid("6862d9de-1e1c-4055-b45e-8fc6845dbc94"), "Revert Transfer Success", "Withdraw", "GlobalEquity", "RevertTransferSuccess", null },
                    { new Guid("6a5113c7-5381-40a8-b49f-b1751c44d22b"), "Name Mismatch (Thai Equity)", "Deposit", "ThaiEquity", "DepositFailedNameMismatch", "Investigate Name" },
                    { new Guid("6e865244-d493-4635-b0f6-7b9b6717d20b"), "Invalid Source", "Deposit", null, "DepositFailedInvalidSource", "Investigate Source of Fund" },
                    { new Guid("723f4edf-fb08-42da-9bd9-60cf7eaead9c"), "Trading Account Deposit Fail", "Deposit", "ThaiEquity", "CashDepositFailed", "Contact Technical Support and manual update Fund to SBA" },
                    { new Guid("76c845bf-fb3a-490f-928c-54811f0a8739"), "FX Failed", "Deposit", "GlobalEquity", "FXFailed", "Refund Required" },
                    { new Guid("77ff1567-da72-4305-a2cc-428ed3f88913"), "OTP Required", "Withdraw", null, "RequestingOtpValidationFailed", null },
                    { new Guid("90c3ae3e-c42b-40cb-8573-0a35096d9272"), "Updating Settrade", "Deposit", "ThaiEquity", "CashDepositWaitingForTradingPlatform", null },
                    { new Guid("975cd24f-b648-402f-a17f-65fd053c9e72"), "Revert Transfer Fail", "Withdraw", "GlobalEquity", "RevertTransferFailed", "Manual Re-allocation Required" },
                    { new Guid("a151f28d-7439-411c-9928-6ca26d7ec82f"), "Fail to Deposit Fund", "Deposit", null, "TransferRequestFailed", "Contact Technical Team" },
                    { new Guid("a8b817d4-a320-4970-973b-5b403b9c8e1a"), "Deposit Completed", "Deposit", "GlobalEquity", "Final", null },
                    { new Guid("c1a73f39-b127-427b-806c-206952194ff4"), "Withdraw Failed - Pending Revert", "Withdraw", "ThaiEquity", "WithdrawalFailed", null },
                    { new Guid("c9e76eeb-f77f-4f8c-ad06-1cd285eca1bd"), "Transfer Request Fail", "Withdraw", "GlobalEquity", "TransferRequestFailed", "Contact Technical Team" },
                    { new Guid("f28492e9-1ee4-4ea7-bfb2-a965eb8cb107"), "Transfer Request Fail", "Withdraw", "ThaiEquity", "TransferRequestFailed", "Contact Technical Team" },
                    { new Guid("f2cc09c8-a739-4797-b00a-492e503f7c8d"), "FX Rate Compare Fail", "Deposit", "GlobalEquity", "FXRateCompareFailed", "Refund Required" },
                    { new Guid("f3092460-34af-4c24-9b87-d24df13a2872"), "Fail to Deposit Fund", "Deposit", null, "DepositFailed", "Contact Technical Team" },
                    { new Guid("f3c9ed99-0978-4797-ae3d-3d0ef7854caa"), "Name Mismatch (Global Equity)", "Deposit", "GlobalEquity", "DepositFailedNameMismatch", "Refund Required" },
                    { new Guid("f4432ba1-6019-44e2-ae9c-c3d3ac64a2ab"), "CCY Allocation Transfer Fail", "Withdraw", "GlobalEquity", "ManualAllocationFailed", "Contact Technical Team" },
                    { new Guid("f8ef36bb-1b95-4405-8263-c8e31c86f5f2"), "Withdraw Completed", "Withdraw", null, "Final", null },
                    { new Guid("fec7cb30-7a63-4248-a7c0-c6d2f9c0cf1a"), "Trading Account Deposit Completed", "Deposit", "ThaiEquity", "CashDepositCompleted", null }
                });

            migrationBuilder.InsertData(
                table: "response_code_actions",
                columns: new[] { "id", "action", "response_code_id" },
                values: new object[,]
                {
                    { new Guid("46d926c8-acb4-46bf-af49-d56bef3da2eb"), "CcyAllocationTransfer", new Guid("0e1158b2-569d-4916-a68c-508c6813cb79") },
                    { new Guid("5939094e-9c8d-4442-9b1e-59bbc4d35c6b"), "CcyAllocationTransfer", new Guid("975cd24f-b648-402f-a17f-65fd053c9e72") },
                    { new Guid("600ee264-c301-4a09-87fb-8e0296ef29be"), "CcyAllocationTransfer", new Guid("f4432ba1-6019-44e2-ae9c-c3d3ac64a2ab") },
                    { new Guid("6fd67285-f01a-4b7d-bf6e-aacf753c4bca"), "CcyAllocationTransfer", new Guid("15a9160b-a5eb-4754-98b2-3cfea4c4e0d2") },
                    { new Guid("74ec6520-a058-4501-b09f-feb9322894c7"), "Approve", new Guid("6a5113c7-5381-40a8-b49f-b1751c44d22b") },
                    { new Guid("7f24e643-3d74-4104-868d-1caeeffb7574"), "Refund", new Guid("6a5113c7-5381-40a8-b49f-b1751c44d22b") },
                    { new Guid("a2a50061-4a3b-4066-a08c-640fa5453bc3"), "Refund", new Guid("222d19bd-92b9-4c40-bcea-3b404a14146a") },
                    { new Guid("c4e2df1b-1525-422b-9810-2485592708a5"), "Refund", new Guid("f3c9ed99-0978-4797-ae3d-3d0ef7854caa") },
                    { new Guid("c921d71f-cf50-4b91-b015-de1d7167747f"), "CcyAllocationTransfer", new Guid("1395482b-939f-46f5-a039-4bd3bdf3edd8") },
                    { new Guid("ccd35bfe-13cc-4871-941c-4e1d3569ba31"), "Refund", new Guid("f2cc09c8-a739-4797-b00a-492e503f7c8d") },
                    { new Guid("d23fdea9-7bb6-4a30-afea-29940ce32615"), "Refund", new Guid("76c845bf-fb3a-490f-928c-54811f0a8739") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
