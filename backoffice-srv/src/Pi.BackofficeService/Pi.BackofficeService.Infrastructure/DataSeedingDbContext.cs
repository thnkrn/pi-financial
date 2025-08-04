#region

using Microsoft.EntityFrameworkCore;
using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using Pi.BackofficeService.Domain.SeedWork;
using Pi.BackofficeService.Infrastructure.EntityConfig;

#endregion

namespace Pi.BackofficeService.Infrastructure;

public class DataSeedingDbContext : DbContext, IUnitOfWork
{
    public DbSet<ResponseCodeAction> ResponseCodeActions => Set<ResponseCodeAction>();
    public DbSet<ResponseCode> ResponseCodes => Set<ResponseCode>();

    public DataSeedingDbContext(DbContextOptions<DataSeedingDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("utf8mb4_0900_ai_ci");
        modelBuilder.ApplyConfiguration<ResponseCode>(new ResponseCodeConfig());
        modelBuilder.ApplyConfiguration<ResponseCodeAction>(new ResponseCodeConfig());

        PopulateResponseCodes(modelBuilder);
    }

    private static void PopulateResponseCodes(ModelBuilder modelBuilder)
    {
        GenerateResponseCodes().ForEach(responseCode =>
        {
            var actions = responseCode.Actions;
            responseCode.Actions = null;

            modelBuilder.Entity<ResponseCode>().HasData(responseCode);
            if (actions == null) return;

            foreach (var responseCodeAction in actions)
            {
                modelBuilder.Entity<ResponseCodeAction>().HasData(new ResponseCodeAction(responseCodeAction.Id,
                    responseCode.Id, responseCodeAction.Action));
            }
        });
    }

    private static List<ResponseCode> GenerateResponseCodes()
    {
        return new List<ResponseCode>()
        {
            #region GE Deposit

            new()
            {
                Id = new Guid("0ac1e94d-990d-4d3c-9fac-8ae42e39351e"),
                Machine = Machine.Deposit,
                ProductType = null,
                State = "DepositWaitingForPayment",
                Description = "Waiting for Fund",
                Suggestion = "Inquire Payment Status",
                Actions = null
            },
            new()
            {
                Id = new Guid("a8b817d4-a320-4970-973b-5b403b9c8e1a"),
                Machine = Machine.Deposit,
                ProductType = ProductType.GlobalEquity,
                State = "Final",
                Description = "Deposit Completed",
                Suggestion = null,
                Actions = null,
                IsFilterable = true
            },
            new()
            {
                Id = new Guid("0e1158b2-569d-4916-a68c-508c6813cb79"),
                Machine = Machine.Deposit,
                ProductType = ProductType.GlobalEquity,
                State = "FXTransferFailed",
                Description = "Manual allocation in XNT",
                Suggestion = "Manual Allocation Required due to failed allocation",
                Actions = GenerateResponseCodeActions(new Dictionary<Guid, Method>()
                {
                    { new Guid("46d926c8-acb4-46bf-af49-d56bef3da2eb"), Method.CcyAllocationTransfer },
                    { new Guid("a679f695-898d-4ba3-94ee-c963a6b38ee7"), Method.ChangeStatusToSuccess },
                    { new Guid("b7b356f2-85c9-4fbe-ac86-7759610e3bc6"), Method.ChangeStatusToFail }
                }),
                IsFilterable = true
            },
            new()
            {
                Id = new Guid("15a9160b-a5eb-4754-98b2-3cfea4c4e0d2"),
                Machine = Machine.Deposit,
                ProductType = ProductType.GlobalEquity,
                State = "FXTransferInsufficientBalance",
                Description = "Insufficient Balance",
                Suggestion = "Alert Finance Team on fund top up",
                Actions = GenerateResponseCodeActions(new Dictionary<Guid, Method>()
                {
                    { new Guid("6fd67285-f01a-4b7d-bf6e-aacf753c4bca"), Method.CcyAllocationTransfer }
                }),
                IsFilterable = true
            },
            new()
            {
                Id = new Guid("a151f28d-7439-411c-9928-6ca26d7ec82f"),
                Machine = Machine.Deposit,
                ProductType = ProductType.GlobalEquity,
                State = "TransferRequestFailed",
                Description = "Transfer Request Failed",
                Suggestion = "Contact Technical Team",
                Actions = null,
                IsFilterable = true
            },
            new()
            {
                Id = new Guid("f3092460-34af-4c24-9b87-d24df13a2872"),
                Machine = Machine.Deposit,
                ProductType = null,
                State = "DepositFailed",
                Description = "Fail to Deposit Fund",
                Suggestion = "Contact Technical Team",
                Actions = null,
                IsFilterable = true
            },
            new()
            {
                Id = new Guid("f3c9ed99-0978-4797-ae3d-3d0ef7854caa"),
                Machine = Machine.Deposit,
                ProductType = ProductType.GlobalEquity,
                State = "DepositFailedNameMismatch",
                Description = "Name Mismatch",
                Suggestion = "Refund Required",
                Actions = GenerateResponseCodeActions(new Dictionary<Guid, Method>()
                {
                    { new Guid("c4e2df1b-1525-422b-9810-2485592708a5"), Method.Refund }
                }),
                IsFilterable = true
            },
            new()
            {
                Id = new Guid("6e865244-d493-4635-b0f6-7b9b6717d20b"),
                Machine = Machine.Deposit,
                ProductType = null,
                State = "DepositFailedInvalidSource",
                Description = "Incorrect Source",
                Suggestion = "Investigate Source of Fund",
                Actions = GenerateResponseCodeActions(new Dictionary<Guid, Method>()
                {
                    { new Guid("524fe165-d582-4f6e-89f6-f815a02309e5"), Method.ChangeStatusToSuccess },
                    { new Guid("fe29778c-4c72-46c1-aaa8-89efb10b6059"), Method.ChangeStatusToFail }
                }),
                IsFilterable = true
            },
            new()
            {
                Id = new Guid("fc8220ca-5728-468f-85f8-406e1c2f0ff4"),
                Machine = Machine.Deposit,
                ProductType = null,
                State = "InvalidSourceSendEmailSuccess",
                Description = "Incorrect Source - Email success",
                Suggestion = "Waiting customer document before proceed next step",
                IsFilterable = true
            },
            new()
            {
                Id = new Guid("8246f2c5-e8dc-4876-956a-9c9fb2361610"),
                Machine = Machine.Deposit,
                ProductType = null,
                State = "InvalidSourceSendEmailFailed",
                Description = "Incorrect Source - Email not success",
                Suggestion = "Manually email to customer for documents before proceed next step",
                IsFilterable = true
            },
            new()
            {
                Id = new Guid("76c845bf-fb3a-490f-928c-54811f0a8739"),
                Machine = Machine.Deposit,
                ProductType = ProductType.GlobalEquity,
                State = "FXFailed",
                Description = "Unable to FX",
                Suggestion = "Refund Required",
                Actions = GenerateResponseCodeActions(new Dictionary<Guid, Method>()
                {
                    { new Guid("d23fdea9-7bb6-4a30-afea-29940ce32615"), Method.Refund },
                    { new Guid("3921769a-c1c7-48a3-9920-f06e9770775b"), Method.ChangeStatusToSuccess },
                    { new Guid("da0a09cb-6003-4879-83e6-24876d3ca863"), Method.ChangeStatusToFail }
                }),
                IsFilterable = true
            },
            new()
            {
                Id = new Guid("f2cc09c8-a739-4797-b00a-492e503f7c8d"),
                Machine = Machine.Deposit,
                ProductType = ProductType.GlobalEquity,
                State = "FXRateCompareFailed",
                Description = "Unfavorable FX (rate over)",
                Suggestion = "Refund Required",
                Actions = GenerateResponseCodeActions(new Dictionary<Guid, Method>()
                {
                    { new Guid("ccd35bfe-13cc-4871-941c-4e1d3569ba31"), Method.Refund },
                    { new Guid("b5ef4ff0-97b8-4d08-8b3f-e215d8fb8f59"), Method.ChangeStatusToSuccess },
                    { new Guid("ccc455ce-0ccb-46fe-a729-371c6aa20b28"), Method.ChangeStatusToFail }
                }),
                IsFilterable = true
            },
            new()
            {
                Id = new Guid("222d19bd-92b9-4c40-bcea-3b404a14146a"),
                Machine = Machine.Deposit,
                ProductType = null,
                State = "DepositFailedAmountMismatch",
                Description = "Amount Mismatch",
                Suggestion = "Refund Required",
                Actions = GenerateResponseCodeActions(new Dictionary<Guid, Method>()
                {
                    { new Guid("a2a50061-4a3b-4066-a08c-640fa5453bc3"), Method.Refund },
                    { new Guid("eb96e5ce-5dfd-4f7b-ae00-66ecefa3c4bb"), Method.ChangeStatusToSuccess },
                    { new Guid("074d958f-1639-417a-a21b-36130c84e39a"), Method.ChangeStatusToFail }
                }),
                IsFilterable = true
            },

            #endregion GE Deposit

            #region GE Withdraw

            new()
            {
                Id = new Guid("f8ef36bb-1b95-4405-8263-c8e31c86f5f2"),
                Machine = Machine.Withdraw,
                ProductType = null,
                State = "Final",
                Description = "Withdraw Completed",
                Suggestion = null,
                Actions = null,
                IsFilterable = true
            },
            new()
            {
                Id = new Guid("c9e76eeb-f77f-4f8c-ad06-1cd285eca1bd"),
                Machine = Machine.Withdraw,
                ProductType = ProductType.GlobalEquity,
                State = "TransferRequestFailed",
                Description = "Transfer Request Fail",
                Suggestion = "Contact Technical Team",
                Actions = null,
                IsFilterable = true
            },
            new()
            {
                Id = new Guid("6862d9de-1e1c-4055-b45e-8fc6845dbc94"),
                Machine = Machine.Withdraw,
                ProductType = ProductType.GlobalEquity,
                State = "RevertTransferSuccess",
                Description = "Revert Transfer Success",
                Suggestion = null,
                Actions = null,
                IsFilterable = true
            },
            new()
            {
                Id = new Guid("975cd24f-b648-402f-a17f-65fd053c9e72"),
                Machine = Machine.Withdraw,
                ProductType = ProductType.GlobalEquity,
                State = "RevertTransferFailed",
                Description = "Manual allocation in XNT",
                Suggestion = "Manual Re-allocation Required",
                Actions = GenerateResponseCodeActions(new Dictionary<Guid, Method>()
                {
                    { new Guid("5939094e-9c8d-4442-9b1e-59bbc4d35c6b"), Method.CcyAllocationTransfer },
                    { new Guid("2e9176ba-9dfe-48ba-836e-92059f1c9488"), Method.ChangeStatusToSuccess },
                    { new Guid("907361e9-fc23-4d25-b32d-9bf12f75a9ec"), Method.ChangeStatusToFail }
                }),
                IsFilterable = true
            },
            new()
            {
                Id = new Guid("77ff1567-da72-4305-a2cc-428ed3f88913"),
                Machine = Machine.Withdraw,
                ProductType = ProductType.GlobalEquity,
                State = "AwaitingOtpValidation",
                Description = "OTP Required",
                Suggestion = null,
                Actions = null
            },

            #endregion

            #region ThaiEquity Withdraw

            new()
            {
                Id = new Guid("23f0b465-57dc-4b07-be8d-9db340bb5cc0"),
                Machine = Machine.Withdraw,
                ProductType = ProductType.ThaiEquity,
                State = "CashWithdrawWaitingForOtpValidation",
                Description = "OTP Required",
                Suggestion = "Waiting for Customer OTP",
                Actions = null
            },
            new()
            {
                Id = new Guid("f28492e9-1ee4-4ea7-bfb2-a965eb8cb107"),
                Machine = Machine.Withdraw,
                ProductType = ProductType.ThaiEquity,
                State = "TransferRequestFailed",
                Description = "Transfer Request Fail",
                Suggestion = "Contact Technical Team and check Customer Trading Account Balance",
                Actions = null,
                IsFilterable = true
            },
            // new()
            // {
            //     Id = new Guid("2258bbbc-2dbf-4519-9d40-3bfa7e4b6609"),
            //     Machine = Machine.Withdraw,
            //     ProductType = ProductType.ThaiEquity,
            //     State = "RevertTransferFailed",
            //     Description = "Revert Transfer Fail",
            //     Suggestion = "Contact Technical Team and check Customer Trading Account Balance",
            //     Actions = null
            // },
            // new()
            // {
            //     Id = new Guid("5afa5a4e-d054-4377-a9f4-e808c1c3706f"),
            //     Machine = Machine.Withdraw,
            //     ProductType = ProductType.ThaiEquity,
            //     State = "RevertTransferSuccess",
            //     Description = "Revert Transfer Success",
            //     Suggestion = null,
            //     Actions = null
            // },
            // new()
            // {
            //     Id = new Guid("c1a73f39-b127-427b-806c-206952194ff4"),
            //     Machine = Machine.Withdraw,
            //     ProductType = ProductType.ThaiEquity,
            //     State = "WithdrawalFailed",
            //     Description = "Withdraw Failed - Pending Revert",
            //     Suggestion = null,
            //     Actions = null
            // },

            #endregion

            #region ThaiEquity Deposit

            new()
            {
                Id = new Guid("60245f07-190e-4c94-b2db-bda11e4f8fa1"),
                Machine = Machine.Deposit,
                ProductType = ProductType.ThaiEquity,
                State = "CashDepositTradingPlatformUpdating",
                Description = "Updating SBA",
                Suggestion = null,
                Actions = null
            },
            new()
            {
                Id = new Guid("3070a7c3-5ef4-4898-b0c2-92efd83f8e9d"),
                Machine = Machine.Deposit,
                ProductType = ProductType.ThaiEquity,
                State = "CashDepositWaitingForGateway",
                Description = "Waiting for SBA Callback",
                Suggestion = null,
                Actions = GenerateResponseCodeActions(
                    new Dictionary<Guid, Method>
                    {
                        { new Guid("518c4ce1-6cda-4b07-911a-bf3d55666e4d"), Method.ChangeStatusToSuccess },
                        { new Guid("c34a1032-49f5-497e-aab7-891529ca7363"), Method.ChangeStatusToFail },
                        { new Guid("04bffc06-1405-4809-b32f-09115d66c08d"), Method.ChangeSetTradeStatusToSuccess },
                        { new Guid("d3b6668f-63e4-40d9-b282-ead905076903"), Method.ChangeSetTradeStatusToFail },
                    })
            },
            new()
            {
                Id = new Guid("90c3ae3e-c42b-40cb-8573-0a35096d9272"),
                Machine = Machine.Deposit,
                ProductType = ProductType.ThaiEquity,
                State = "CashDepositWaitingForTradingPlatform",
                Description = "Updating Settrade",
                Suggestion = null,
                Actions = null
            },
            new()
            {
                Id = new Guid("fec7cb30-7a63-4248-a7c0-c6d2f9c0cf1a"),
                Machine = Machine.Deposit,
                ProductType = ProductType.ThaiEquity,
                State = "CashDepositCompleted",
                Description = "Deposit Completed",
                Suggestion = null,
                Actions = null,
                IsFilterable = true
            },
            new()
            {
                Id = new Guid("723f4edf-fb08-42da-9bd9-60cf7eaead9c"),
                Machine = Machine.Deposit,
                ProductType = null,
                State = "CashDepositFailed",
                Description = "Trading Account Deposit Fail",
                Suggestion = "Check Customer Trading Account Balance, before Manual Allocate",
                Actions = GenerateResponseCodeActions(
                    new Dictionary<Guid, Method>
                    {
                        { new Guid("9462554b-f394-4572-b29f-0e5830b03889"), Method.RetrySbaDeposit },
                        { new Guid("ac2b76d7-37c4-45af-a796-d8ecce065baf"), Method.ChangeStatusToSuccess },
                        { new Guid("d69c72b1-7eda-4fc6-9bd8-5fcc308444d0"), Method.ChangeStatusToFail },
                        { new Guid("1e5f4d1c-f65b-4033-ad7d-0a5c3196d3e5"), Method.ChangeSetTradeStatusToSuccess },
                        { new Guid("45828eb2-052a-4c36-9c95-c4268077817e"), Method.ChangeSetTradeStatusToFail },
                    }),
                IsFilterable = true
            },
            new()
            {
                Id = new Guid("6a5113c7-5381-40a8-b49f-b1751c44d22b"),
                Machine = Machine.Deposit,
                ProductType = ProductType.ThaiEquity,
                State = "DepositFailedNameMismatch",
                Description = "Name Mismatch",
                Suggestion = "Investigate Name",
                Actions = GenerateResponseCodeActions(
                    new Dictionary<Guid, Method>()
                    {
                        { new Guid("74ec6520-a058-4501-b09f-feb9322894c7"), Method.Approve },
                        { new Guid("7f24e643-3d74-4104-868d-1caeeffb7574"), Method.Refund },
                        { new Guid("f938909d-7b69-473b-93a1-4b2b171a2fd5"), Method.ChangeStatusToSuccess },
                        { new Guid("d7cb0c26-e1f8-43e9-ba4f-6449789535d6"), Method.ChangeStatusToFail }
                    }),
                IsFilterable = true
            },
            new()
            {
                Id = new Guid("aa6e979e-7304-4731-b535-52c3ec111ca4"),
                Machine = Machine.Deposit,
                ProductType = ProductType.ThaiEquity,
                State = "BillPaymentFailedNameMismatch",
                Description = "Name Mismatch",
                Suggestion = "Investigate Name",
                Actions = GenerateResponseCodeActions(
                    new Dictionary<Guid, Method>()
                    {
                        { new Guid("651698ea-702a-41df-883e-fb9e56691264"), Method.Approve },
                        { new Guid("124f57f9-d44b-41e3-adba-5d75f9f26ae1"), Method.ChangeStatusToSuccess },
                        { new Guid("7045ca97-1700-4d5e-a8e7-51aa67ea8ea1"), Method.ChangeStatusToFail }
                    }),
                IsFilterable = true
            },
            new()
            {
                Id = new Guid("e7267f12-7e3d-4181-8bf2-ef87e8903842"),
                Machine = Machine.Deposit,
                ProductType = null,
                State = "UpBackFailedPurposeMismatch",
                Description = "Pending for manual in SBA",
                Suggestion = "manual in SBA",
                Actions = GenerateResponseCodeActions(
                    new Dictionary<Guid, Method>()
                    {
                        { new Guid("8ff5de2a-d090-4648-8482-dd096f104259"), Method.ChangeStatusToSuccess },
                        { new Guid("1400e565-e420-40ed-88b7-2f300590e14c"), Method.ChangeStatusToFail }
                    }),
                IsFilterable = true
            },
            #endregion

            #region Backoffice Services
            
            new()
            {
                Id = new Guid("06b0657c-9338-4db1-baf0-95351bec3de2"),
                Machine = Machine.Deposit,
                ProductType = ProductType.GlobalEquity,
                State = "Refunding",
                Description = "Refunding",
                Suggestion = null,
                Actions = null
            },
            new()
            {
                Id = new Guid("b6bedce3-9801-419a-8b3d-e1b726ba9607"),
                Machine = Machine.Deposit,
                ProductType = null,
                State = "RefundSuccess",
                Description = "Refund Success",
                Suggestion = null,
                Actions = null,
                IsFilterable = true
            },
            new()
            {
                Id = new Guid("d53c7aca-bed8-409b-a163-40f33180960d"),
                Machine = Machine.Deposit,
                ProductType = null,
                State = "DepositRefunding",
                Description = "Refunding",
                Suggestion = null,
                Actions = null
            },
            new()
            {
                Id = new Guid("1395482b-939f-46f5-a039-4bd3bdf3edd8"),
                Machine = Machine.Deposit,
                ProductType = ProductType.GlobalEquity,
                State = "ManualAllocationFailed",
                Description = "Manual Allocation in XNT Failed",
                Suggestion = "Manual Re-allocation Required",
                Actions = GenerateResponseCodeActions(new Dictionary<Guid, Method>()
                {
                    { new Guid("c921d71f-cf50-4b91-b015-de1d7167747f"), Method.CcyAllocationTransfer }
                }),
                IsFilterable = true
            },
            new()
            {
                Id = new Guid("f4432ba1-6019-44e2-ae9c-c3d3ac64a2ab"),
                Machine = Machine.Withdraw,
                ProductType = ProductType.GlobalEquity,
                State = "ManualAllocationFailed",
                Description = "Manual Allocation in XNT Failed",
                Suggestion = "Manual Re-allocation Required",
                Actions = GenerateResponseCodeActions(new Dictionary<Guid, Method>()
                {
                    { new Guid("600ee264-c301-4a09-87fb-8e0296ef29be"), Method.CcyAllocationTransfer }
                }),
                IsFilterable = true
            },

            #endregion

            #region Wallet-V2

            new()
            {
                Id = new Guid("f534e848-9441-40bb-8878-1b9b5b3a801f"),
                Machine = Machine.Deposit,
                ProductType = null,
                State = "UpBackFailedRequireActionSba",
                Description = "Trading Account Deposit Fail",
                Suggestion = "Check Customer Trading Account Balance, before Manual Allocate",
                Actions = GenerateResponseCodeActions(
                    new Dictionary<Guid, Method>
                    {
                        { new Guid("bf0c2940-1080-453d-a7ca-6718a880cda2"), Method.RetrySbaDeposit },
                        { new Guid("0f2ef9b5-6c84-4ceb-b794-b74e12bba61e"), Method.ChangeStatusToSuccess },
                        { new Guid("0d201e4b-94f8-42aa-b708-a618cbbb14d8"), Method.ChangeStatusToFail }
                    }),
                IsFilterable = true
            },
            new()
            {
                Id = new Guid("f9e00911-c580-48f2-9302-d7e10388507f"),
                Machine = Machine.Deposit,
                ProductType = null,
                State = "UpBackFailedRequireActionSetTrade",
                Description = "SetTrade Trading Account Deposit Fail",
                Suggestion = "Check MT4, Check Customer SetTrade Account Balance, before Manual Allocate",
                Actions = GenerateResponseCodeActions(
                    new Dictionary<Guid, Method>
                    {
                        { new Guid("b0c5da2a-0526-41b1-a5ee-bc6ab2b5090a"), Method.RetrySetTradeDeposit },
                        { new Guid("744ba0f4-040b-4bb0-ab4e-54cd21f2a4e5"), Method.ChangeStatusToSuccess },
                        { new Guid("a55878c9-9a74-42bb-85c4-fb27298730bf"), Method.ChangeStatusToFail }
                    }),
                IsFilterable = true
            },
            new()
            {
                Id = new Guid("b05dbb35-7e00-4ada-9811-4ece7d7e1625"),
                Machine = Machine.Deposit,
                ProductType = null,
                State = "WaitingForPayment",
                Description = "Waiting for receiving kkp response",
                Suggestion = "Check kkp report, Check Customer Trading Account balance,  before kkp callback confirm",
                Actions = GenerateResponseCodeActions(
                    new Dictionary<Guid, Method>
                    {
                        { new Guid("eb19e2ce-4565-4ea5-9c1b-177f14f582cb"), Method.DepositKkpConfirm },
                    }),
                IsFilterable = true
            },
            new()
            {
                Id = new Guid("d9fef26d-a2ef-470e-82be-9f64f2c32d90"),
                Machine = Machine.Deposit,
                ProductType = null,
                State = "DepositWaitingForGateway",
                Description = "Waiting for receiving freewill response",
                Suggestion = "Check freewill, Check Customer Trading Account balance , Before confirm SBA",
                Actions = GenerateResponseCodeActions(
                    new Dictionary<Guid, Method>
                    {
                        { new Guid("b89a1eba-eedd-4509-b427-2c26456f2755"), Method.SbaConfirm },
                        { new Guid("8303a6a4-657e-4937-a8f5-8c2f6c58958d"), Method.ChangeStatusToSuccess },
                        { new Guid("2a3eff05-64bd-4a4b-a58e-aef6ffbcd80a"), Method.ChangeStatusToFail }
                    }),
                IsFilterable = true
            },
            new()
            {
                Id = new Guid("caa1b189-aa54-4c5d-933f-8eb8bb066aa2"),
                Machine = Machine.Deposit,
                ProductType = null,
                State = "WaitingForAtsGatewayConfirmation",
                Description = "Waiting for ats response",
                Suggestion = "Check freewill report, before approve front",
                Actions = GenerateResponseCodeActions(
                    new Dictionary<Guid, Method>
                    {
                        { new Guid("586c694a-dfb1-41b2-af6b-b2ff1fe63fbd"), Method.SbaDepositAtsCallbackConfirm },
                        { new Guid("f797b251-e55b-4077-99cf-4f4268788754"), Method.ChangeStatusToSuccess },
                        { new Guid("9807126c-e2b4-4b83-a530-579a5a291a36"), Method.ChangeStatusToFail }
                    }),
                IsFilterable = true
            },
            new()
            {
                Id = new Guid("3dc91a3c-402b-45b5-80a5-b96c75e24391"),
                Machine = Machine.Withdraw,
                ProductType = null,
                State = "WaitingForAtsGatewayConfirmation",
                Description = "Waiting for ats response",
                Suggestion = "Change transaction status",
                Actions = GenerateResponseCodeActions(
                    new Dictionary<Guid, Method>
                    {
                        { new Guid("a01bf92f-f69d-4a01-9349-325a2d2a296b"), Method.ChangeStatusToSuccess },
                        { new Guid("15cf9ccf-054e-4061-8c8d-bc4e602191e0"), Method.ChangeStatusToFail }
                    }),
                IsFilterable = true
            },
            new()
            {
                Id = new Guid("4bc8fc98-a84c-4bc0-97cc-99cca7c24e5b"),
                Machine = Machine.Withdraw,
                ProductType = null,
                State = "WithdrawFailedRequireActionRecovery",
                Description = "Pending Revert or Retry Transaction",
                Suggestion = "Check KKP report. Retry withdrawal, revert transaction, or change status to success if withdrawn",
                Actions = GenerateResponseCodeActions(
                    new Dictionary<Guid, Method>
                    {
                        { new Guid("cddf81a7-c5f7-434d-870a-198d653b83dc"), Method.ChangeStatusToSuccess },
                        { new Guid("a6f84686-6ee2-432f-955f-fa8c570b210e"), Method.ChangeStatusToFail },
                        { new Guid("6c0ed478-858d-481f-815b-3ad9cf1355ec"), Method.RetryKkpWithdraw }
                    }),
                IsFilterable = true
            },
            new()
            {
                Id = new Guid("c0061a15-f868-42f5-b8d2-52bd886aac3b"),
                Machine = Machine.Withdraw,
                ProductType = null,
                State = "UpBackFailedSetTrade",
                Description = "Pending Revert or Retry Transaction",
                Suggestion = "Check SetTrade. Retry withdrawal or revert transaction before mark as fail",
                Actions = GenerateResponseCodeActions(
                    new Dictionary<Guid, Method>
                    {
                        { new Guid("7caf8a8b-aa4c-497a-9869-197a6ba0190d"), Method.ChangeStatusToSuccess },
                        { new Guid("e4cafe96-c013-4e4c-8c65-a49f0c0bd353"), Method.ChangeStatusToFail },
                        { new Guid("2339a6b4-a6c9-4be3-8746-d677d8db70ff"), Method.RetrySetTradeWithdraw }
                    }),
                IsFilterable = true
            },
            new()
            {
                Id = new Guid("3fedca1a-e66c-4964-ad31-b40d425b854a"),
                Machine = Machine.Withdraw,
                ProductType = null,
                State = "UpBackFailedFreewill",
                Description = "Pending Revert or Retry Transaction",
                Suggestion = "Check Freewill. Retry withdrawal or revert transaction before mark as fail",
                Actions = GenerateResponseCodeActions(
                    new Dictionary<Guid, Method>
                    {
                        { new Guid("fb7a25e9-1035-43f5-b51a-fbad9eb52cf5"), Method.ChangeStatusToSuccess },
                        { new Guid("b972d582-ce0b-46b7-87d5-4ec07239e0ce"), Method.ChangeStatusToFail },
                        { new Guid("085d3732-3729-4d2a-a254-021fecb4a71e"), Method.RetrySbaWithdraw }
                    }),
                IsFilterable = true
            },
            new()
            {
                Id = new Guid("532d2838-9610-4704-a267-4e609032adf9"),
                Machine = Machine.Withdraw,
                ProductType = null,
                State = "UpBackFailedRequireActionRevert",
                Description = "Pending Revert Transaction",
                Suggestion = "Check freewill or settrade, Contact IT to revert, Before mark as fail",
                Actions = GenerateResponseCodeActions(
                    new Dictionary<Guid, Method>
                    {
                        { new Guid("5f747a23-eb7e-4dba-8442-38b9b4cb6fa1"), Method.ChangeStatusToSuccess },
                        { new Guid("4272eef0-736c-4f3a-bb81-5f7e2391f789"), Method.ChangeStatusToFail }
                    }),
                IsFilterable = true
            },
            new()
            {
                Id = new Guid("d8fa6460-36cf-458f-9f12-f43cdd9c6b28"),
                Machine = Machine.Withdraw,
                ProductType = null,
                State = "WithdrawWaitingForGateway",
                Description = "Waiting for receiving freewill response",
                Suggestion = "Check freewill, Before confirm SBA",
                Actions = GenerateResponseCodeActions(
                    new Dictionary<Guid, Method>
                    {
                        { new Guid("b06c1068-7d65-4784-822f-5c471a89a21d"), Method.SbaConfirm },
                        { new Guid("6d98f3cf-5654-44f8-850f-7cca6257af00"), Method.ChangeStatusToSuccess },
                        { new Guid("b43288c1-b3e1-413b-a0e2-fe52781ee1b0"), Method.ChangeStatusToFail }
                    }),
                IsFilterable = true
            },
            new()
            {
                Id = new Guid("fe785ac2-09af-4cb0-a679-030310809bee"),
                Machine = Machine.Deposit,
                ProductType = null,
                State = "TfexCashDepositFailed",
                Description = "SetTrade Trading Account Deposit Fail",
                Suggestion = "Change transaction status",
                Actions = GenerateResponseCodeActions(
                    new Dictionary<Guid, Method>
                    {
                        { new Guid("aadb8248-f18d-438e-bd9e-c33753e71fee"), Method.ChangeStatusToSuccess },
                        { new Guid("103152af-c45c-462e-8eb8-e3b0cf77990e"), Method.ChangeStatusToFail },
                        { new Guid("f9ba7739-b55c-49de-8beb-ccc3d08beba8"), Method.ChangeSetTradeStatusToSuccess },
                        { new Guid("ab6bab02-cd95-4ac1-846f-c3acfcc39541"), Method.ChangeSetTradeStatusToFail },
                    }),
                IsFilterable = true
            },
            new()
            {
                Id = new Guid("961c1faf-963c-40ed-87e1-01ebfdebb0a0"),
                Machine = Machine.Deposit,
                ProductType = null,
                State = string.Empty,
                Description = "Update Transaction",
                Suggestion = null!,
                Actions = GenerateResponseCodeActions(
                    new Dictionary<Guid, Method>
                    {
                        { new Guid("f1f75678-b77f-4091-8267-cc284484cb0c"), Method.ChangeStatusToSuccess },
                        { new Guid("22872725-cdba-4ab8-9b04-c2748be6e2cb"), Method.ChangeStatusToFail },
                    }),
                IsFilterable = false
            },
            new()
            {
                Id = new Guid("bba989b7-8103-4841-802b-acc9146b0bd9"),
                Machine = Machine.Withdraw,
                ProductType = null,
                State = string.Empty,
                Description = "Update Transaction",
                Suggestion = null!,
                Actions = GenerateResponseCodeActions(
                    new Dictionary<Guid, Method>
                    {
                        { new Guid("b4bab9c1-fd63-4b41-83d6-2465226ed4d6"), Method.ChangeStatusToSuccess },
                        { new Guid("362f0108-15c5-4bba-899f-92fd33a6f6ae"), Method.ChangeStatusToFail },
                    }),
                IsFilterable = false
            },
            new()
            {
                Id = new Guid("a10a5dfb-265b-457d-996b-4858ad450bd5"),
                Machine = Machine.Deposit,
                ProductType = null,
                State = "BillPaymentRequestInvalid",
                Description = "Account Number Mismatch (Ref1)",
                Suggestion = "Look up sender bank name and verify with customers before input correct account number",
                Actions = GenerateResponseCodeActions(
                    new Dictionary<Guid, Method>
                    {
                        { new Guid("c54b668a-32a0-478f-bd57-ae40fda48a00"), Method.ChangeStatusToSuccess },
                        { new Guid("fe1f553b-c87e-43ec-868d-d19cd05f6e28"), Method.ChangeStatusToFail },
                        { new Guid("6952e94c-38aa-44eb-9f2b-9fb2fb280907"), Method.UpdateBillPaymentReference },
                    }),
                IsFilterable = false
            },
            #endregion

            #region Transfer Cash
            
            new()
            {
                Id = new Guid("f4c3f9ab-1873-4b1b-b301-9002b5aaf85f"),
                Machine = Machine.TransferCash,
                ProductType = null,
                State = "TransferCashFailedRequireActionSetTrade",
                Description = "SetTrade Trading Account Transfer Fail",
                Suggestion = "Check MT4, check customer SetTrade account balance, before manual allocate",
                Actions = GenerateResponseCodeActions(
                    new Dictionary<Guid, Method>
                    {
                        { new Guid("0c08be70-9370-44ba-bd4a-8e6404392a2f"), Method.ChangeStatusToSuccess },
                        { new Guid("53cd518f-6f3a-4d7d-b211-097fce7873a5"), Method.ChangeStatusToFail },
                    }),
                IsFilterable = false
            },
            new()
            {
                Id = new Guid("9621da04-db2a-4286-a792-8b7000897f43"),
                Machine = Machine.TransferCash,
                ProductType = null,
                State = "TransferFailedRequireActionSetTrade",
                Description = "SetTrade Trading Account Transfer Fail",
                Suggestion = "Check MT4, check customer SetTrade account balance, before manual allocate",
                Actions = GenerateResponseCodeActions(
                    new Dictionary<Guid, Method>
                    {
                        { new Guid("5c329989-d81e-4a81-8e4c-140de3454417"), Method.ChangeStatusToSuccess },
                        { new Guid("641da5c8-cf45-4f3c-812e-c9b13047bca0"), Method.ChangeStatusToFail },
                    }),
                IsFilterable = false
            },
            #endregion
        };
    }

    private static List<ResponseCodeAction> GenerateResponseCodeActions(Dictionary<Guid, Method> actions)
    {
        return actions.Select(action => new ResponseCodeAction(action.Key, Guid.NewGuid(), action.Value))
            .ToList();
    }
}