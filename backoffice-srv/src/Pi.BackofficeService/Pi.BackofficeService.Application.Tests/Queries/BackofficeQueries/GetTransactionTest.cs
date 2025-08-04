using Moq;
using Pi.BackofficeService.API.Models;
using Pi.BackofficeService.Application.Queries;
using Pi.BackofficeService.Application.Services.UserService;
using Pi.BackofficeService.Application.Services.WalletService;
using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransferCashAggregate;
using TransactionFilterRequest = Pi.BackofficeService.API.Models.TransactionFilterRequest;

namespace Pi.BackofficeService.Application.Tests.Queries.BackofficeQueries;

public class GetTransactionsTest
{
    private readonly Mock<IDepositWithdrawService> _depositWithdrawService;
    private readonly Mock<ITransferCashService> _transferCashService;
    private readonly Mock<ITransactionRepository> _transactionRepository;
    private readonly Mock<IResponseCodeRepository> _responseCodeRepository;
    private readonly Mock<IResponseCodeActionRepository> _responseCodeActionRepository;
    private readonly Application.Queries.BackofficeQueries _query;

    public GetTransactionsTest()
    {
        _depositWithdrawService = new Mock<IDepositWithdrawService>();
        _transferCashService = new Mock<ITransferCashService>();
        _transactionRepository = new Mock<ITransactionRepository>();
        _responseCodeRepository = new Mock<IResponseCodeRepository>();
        _responseCodeActionRepository = new Mock<IResponseCodeActionRepository>();

        _query = new Application.Queries.BackofficeQueries(_depositWithdrawService.Object, _transferCashService.Object, _transactionRepository.Object,
            _responseCodeRepository.Object, _responseCodeActionRepository.Object);
    }

    [Theory]
    [InlineData("Success")]
    [InlineData("Fail")]
    [InlineData("Processing")]
    [InlineData("Pending")]
    [InlineData(null)]
    public async Task GetTransactionV2Paginate_ShouldReturnCorrectResponse(string status)
    {
        var expected = GetExpectedPaginateResponse();
        SetupGetTransactionV2(expected);

        var transactionPaginateRequest = new TransactionPaginateRequest(null, null,
            new TransactionFilterRequest(Channel.QR, null, ProductType.ThaiEquity, null, null, null, null, null, null, null, status, null, null, null,
                null, null, null), 1, 20);
        var filters = new TransactionFilter(null, TransactionType.Deposit, null, null, ResponseCodeId: Guid.NewGuid(), null, null, null, null, null, status, null, null, null, null, null, null);

        var actual = await _query.GetTransactionsV2Paginate(transactionPaginateRequest.Page, transactionPaginateRequest.PageSize, null, null, filters);

        Assert.Equal(expected.Data.FirstOrDefault()?.AccountCode, actual.Records[0].Transaction.AccountCode);
        Assert.Equal(expected.Data.FirstOrDefault()?.CustomerName, actual.Records[0].Transaction.CustomerName);
        Assert.Equal(expected.Data.FirstOrDefault()?.BankAccount, actual.Records[0].Transaction.BankAccount);
        Assert.Equal(expected.Data.FirstOrDefault()?.BankAccountNo, actual.Records[0].Transaction.BankAccountNo);
        Assert.Equal(expected.Data.FirstOrDefault()?.BankAccountName, actual.Records[0].Transaction.BankAccountName);
        Assert.Equal(expected.Data.FirstOrDefault()?.Status, actual.Records[0].Transaction.Status);
    }

    [Fact]
    public async Task GetTransactionV2ByTransactionNo_ShouldReturnCorrectResponse()
    {
        var expected = GetExpectedResponse("DHDP2000010100001");
        SetupGetTransactionV2(expected);

        var actual = await _query.GetTransactionV2ByTransactionNo("DHDP2000010100001");

        Assert.Equal(expected?.AccountCode, actual?.Transaction.AccountCode);
        Assert.Equal(expected?.CustomerName, actual?.Transaction.CustomerName);
        Assert.Equal(expected?.BankAccount, actual?.Transaction.BankAccount);
        Assert.Equal(expected?.BankAccountNo, actual?.Transaction.BankAccountNo);
        Assert.Equal(expected?.BankAccountName, actual?.Transaction.BankAccountName);
        Assert.Equal(expected?.Status, actual?.Transaction.Status);

        // Check QrDeposit
        Assert.Equal(expected?.QrDeposit?.State, actual?.Transaction.QrDeposit?.State);
        Assert.Equal(expected?.QrDeposit?.PaymentReceivedAmount, actual?.Transaction.QrDeposit?.PaymentReceivedAmount);
        Assert.Equal(expected?.QrDeposit?.PaymentReceivedDateTime, actual?.Transaction.QrDeposit?.PaymentReceivedDateTime);
        Assert.Equal(expected?.QrDeposit?.Fee, actual?.Transaction.QrDeposit?.Fee);
        Assert.Equal(expected?.QrDeposit?.CreatedAt, actual?.Transaction.QrDeposit?.CreatedAt);
        Assert.Equal(expected?.QrDeposit?.UpdatedAt, actual?.Transaction.QrDeposit?.UpdatedAt);

        // Check OddDeposit
        Assert.Equal(expected?.OddDeposit?.State, actual?.Transaction.OddDeposit?.State);
        Assert.Equal(expected?.OddDeposit?.PaymentReceivedAmount, actual?.Transaction.OddDeposit?.PaymentReceivedAmount);
        Assert.Equal(expected?.OddDeposit?.PaymentReceivedDateTime, actual?.Transaction.OddDeposit?.PaymentReceivedDateTime);
        Assert.Equal(expected?.OddDeposit?.Fee, actual?.Transaction.OddDeposit?.Fee);
        Assert.Equal(expected?.OddDeposit?.CreatedAt, actual?.Transaction.OddDeposit?.CreatedAt);
        Assert.Equal(expected?.OddDeposit?.UpdatedAt, actual?.Transaction.OddDeposit?.UpdatedAt);

        // Check AtsDeposit
        Assert.Equal(expected?.AtsDeposit?.State, actual?.Transaction.AtsDeposit?.State);
        Assert.Equal(expected?.AtsDeposit?.PaymentReceivedAmount, actual?.Transaction.AtsDeposit?.PaymentReceivedAmount);
        Assert.Equal(expected?.AtsDeposit?.PaymentReceivedDateTime, actual?.Transaction.AtsDeposit?.PaymentReceivedDateTime);
        Assert.Equal(expected?.AtsDeposit?.Fee, actual?.Transaction.AtsDeposit?.Fee);
        Assert.Equal(expected?.AtsDeposit?.CreatedAt, actual?.Transaction.AtsDeposit?.CreatedAt);
        Assert.Equal(expected?.AtsDeposit?.UpdatedAt, actual?.Transaction.AtsDeposit?.UpdatedAt);

        // Check OddWithdraw
        Assert.Equal(expected?.OddWithdraw?.State, actual?.Transaction.OddWithdraw?.State);
        Assert.Equal(expected?.OddWithdraw?.PaymentDisbursedAmount, actual?.Transaction.OddWithdraw?.PaymentDisbursedAmount);
        Assert.Equal(expected?.OddWithdraw?.PaymentDisbursedDateTime, actual?.Transaction.OddWithdraw?.PaymentDisbursedDateTime);
        Assert.Equal(expected?.OddWithdraw?.Fee, actual?.Transaction.OddWithdraw?.Fee);
        Assert.Equal(expected?.OddWithdraw?.CreatedAt, actual?.Transaction.OddWithdraw?.CreatedAt);
        Assert.Equal(expected?.OddWithdraw?.UpdatedAt, actual?.Transaction.OddWithdraw?.UpdatedAt);

        // Check AtsWithdraw
        Assert.Equal(expected?.AtsWithdraw?.State, actual?.Transaction.AtsWithdraw?.State);
        Assert.Equal(expected?.AtsWithdraw?.PaymentDisbursedAmount, actual?.Transaction.AtsWithdraw?.PaymentDisbursedAmount);
        Assert.Equal(expected?.AtsWithdraw?.PaymentDisbursedDateTime, actual?.Transaction.AtsWithdraw?.PaymentDisbursedDateTime);
        Assert.Equal(expected?.AtsWithdraw?.Fee, actual?.Transaction.AtsWithdraw?.Fee);
        Assert.Equal(expected?.AtsWithdraw?.CreatedAt, actual?.Transaction.AtsWithdraw?.CreatedAt);
        Assert.Equal(expected?.AtsWithdraw?.UpdatedAt, actual?.Transaction.AtsWithdraw?.UpdatedAt);

        // Check GlobalTransfer
        Assert.Equal(expected?.GlobalTransfer?.State, actual?.Transaction.GlobalTransfer?.State);
        Assert.Equal(expected?.GlobalTransfer?.GlobalAccount, actual?.Transaction.GlobalTransfer?.GlobalAccount);
        Assert.Equal(expected?.GlobalTransfer?.RequestedCurrency, actual?.Transaction.GlobalTransfer?.RequestedCurrency);
        Assert.Equal(expected?.GlobalTransfer?.RequestedFxRate, actual?.Transaction.GlobalTransfer?.RequestedFxRate);
        Assert.Equal(expected?.GlobalTransfer?.RequestedFxCurrency, actual?.Transaction.GlobalTransfer?.RequestedFxCurrency);
        Assert.Equal(expected?.GlobalTransfer?.PaymentReceivedAmount, actual?.Transaction.GlobalTransfer?.PaymentReceivedAmount);
        Assert.Equal(expected?.GlobalTransfer?.PaymentReceivedCurrency, actual?.Transaction.GlobalTransfer?.PaymentReceivedCurrency);
        Assert.Equal(expected?.GlobalTransfer?.FxInitiateRequestDateTime, actual?.Transaction.GlobalTransfer?.FxInitiateRequestDateTime);
        Assert.Equal(expected?.GlobalTransfer?.FxTransactionId, actual?.Transaction.GlobalTransfer?.FxTransactionId);
        Assert.Equal(expected?.GlobalTransfer?.FxConfirmedAmount, actual?.Transaction.GlobalTransfer?.FxConfirmedAmount);
        Assert.Equal(expected?.GlobalTransfer?.FxConfirmedExchangeRate, actual?.Transaction.GlobalTransfer?.FxConfirmedExchangeRate);
        Assert.Equal(expected?.GlobalTransfer?.FxConfirmedCurrency, actual?.Transaction.GlobalTransfer?.FxConfirmedCurrency);
        Assert.Equal(expected?.GlobalTransfer?.FxConfirmedDateTime, actual?.Transaction.GlobalTransfer?.FxConfirmedDateTime);
        Assert.Equal(expected?.GlobalTransfer?.TransferAmount, actual?.Transaction.GlobalTransfer?.TransferAmount);
        Assert.Equal(expected?.GlobalTransfer?.TransferCurrency, actual?.Transaction.GlobalTransfer?.TransferCurrency);
        Assert.Equal(expected?.GlobalTransfer?.TransferFee, actual?.Transaction.GlobalTransfer?.TransferFee);
        Assert.Equal(expected?.GlobalTransfer?.TransferFromAccount, actual?.Transaction.GlobalTransfer?.TransferFromAccount);
        Assert.Equal(expected?.GlobalTransfer?.TransferToAccount, actual?.Transaction.GlobalTransfer?.TransferToAccount);
        Assert.Equal(expected?.GlobalTransfer?.TransferRequestTime, actual?.Transaction.GlobalTransfer?.TransferRequestTime);
        Assert.Equal(expected?.GlobalTransfer?.TransferCompleteTime, actual?.Transaction.GlobalTransfer?.TransferCompleteTime);
        Assert.Equal(expected?.GlobalTransfer?.FailedReason, actual?.Transaction.GlobalTransfer?.FailedReason);
        Assert.Equal(expected?.GlobalTransfer?.CreatedAt, actual?.Transaction.GlobalTransfer?.CreatedAt);
        Assert.Equal(expected?.GlobalTransfer?.UpdatedAt, actual?.Transaction.GlobalTransfer?.UpdatedAt);

        // Check UpBack
        Assert.Equal(expected?.UpBack?.State, actual?.Transaction.UpBack?.State);
        Assert.Equal(expected?.UpBack?.FailedReason, actual?.Transaction.UpBack?.FailedReason);
        Assert.Equal(expected?.UpBack?.CreatedAt, actual?.Transaction.UpBack?.CreatedAt);
        Assert.Equal(expected?.UpBack?.UpdatedAt, actual?.Transaction.UpBack?.UpdatedAt);

        // Check Recovery
        Assert.Equal(expected?.Recovery?.State, actual?.Transaction.Recovery?.State);
        Assert.Equal(expected?.Recovery?.GlobalAccount, actual?.Transaction.Recovery?.GlobalAccount);
        Assert.Equal(expected?.Recovery?.TransferFromAccount, actual?.Transaction.Recovery?.TransferFromAccount);
        Assert.Equal(expected?.Recovery?.TransferAmount, actual?.Transaction.Recovery?.TransferAmount);
        Assert.Equal(expected?.Recovery?.TransferToAccount, actual?.Transaction.Recovery?.TransferToAccount);
        Assert.Equal(expected?.Recovery?.TransferCurrency, actual?.Transaction.Recovery?.TransferCurrency);
        Assert.Equal(expected?.Recovery?.TransferRequestTime, actual?.Transaction.Recovery?.TransferRequestTime);
        Assert.Equal(expected?.Recovery?.TransferCompleteTime, actual?.Transaction.Recovery?.TransferCompleteTime);
        Assert.Equal(expected?.Recovery?.FailedReason, actual?.Transaction.Recovery?.FailedReason);
        Assert.Equal(expected?.Recovery?.CreatedAt, actual?.Transaction.Recovery?.CreatedAt);
        Assert.Equal(expected?.Recovery?.UpdatedAt, actual?.Transaction.Recovery?.UpdatedAt);

        // Check Refund
        Assert.Equal(expected?.Refund?.RefundId, actual?.Transaction.Refund?.RefundId);
        Assert.Equal(expected?.Refund?.Amount, actual?.Transaction.Refund?.Amount);
        Assert.Equal(expected?.Refund?.TransferToAccountNo, actual?.Transaction.Refund?.TransferToAccountNo);
        Assert.Equal(expected?.Refund?.TransferToAccountName, actual?.Transaction.Refund?.TransferToAccountName);
        Assert.Equal(expected?.Refund?.Fee, actual?.Transaction.Refund?.Fee);
        Assert.Equal(expected?.Refund?.CreatedAt, actual?.Transaction.Refund?.CreatedAt);

        // Check BillPayment
        Assert.Equal(expected?.BillPayment?.State, actual?.Transaction.BillPayment?.State);
        Assert.Equal(expected?.BillPayment?.PaymentReceivedAmount, actual?.Transaction.BillPayment?.PaymentReceivedAmount);
        Assert.Equal(expected?.BillPayment?.PaymentReceivedDateTime, actual?.Transaction.BillPayment?.PaymentReceivedDateTime);
        Assert.Equal(expected?.BillPayment?.Reference1, actual?.Transaction.BillPayment?.Reference1);
        Assert.Equal(expected?.BillPayment?.Reference2, actual?.Transaction.BillPayment?.Reference2);
        Assert.Equal(expected?.BillPayment?.CustomerPaymentName, actual?.Transaction.BillPayment?.CustomerPaymentName);
        Assert.Equal(expected?.BillPayment?.CustomerPaymentBankCode, actual?.Transaction.BillPayment?.CustomerPaymentBankCode);
        Assert.Equal(expected?.BillPayment?.Fee, actual?.Transaction.BillPayment?.Fee);
        Assert.Equal(expected?.BillPayment?.BillPaymentTransactionRef, actual?.Transaction.BillPayment?.BillPaymentTransactionRef);
        Assert.Equal(expected?.BillPayment?.FailedReason, actual?.Transaction.BillPayment?.FailedReason);
    }

    [Theory]
    [InlineData("Success")]
    [InlineData("Fail")]
    [InlineData("Processing")]
    [InlineData("Pending")]
    [InlineData(null)]
    public async Task GetTransferCashPaginate_ShouldReturnResponseCode(string status)
    {
        var expected = GetExpectedTransferCashPaginateResponse();
        _transferCashService.Setup(s =>
                s.GetTransferCashHistory(It.IsAny<TransferCashTransactionFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);
        _responseCodeRepository.Setup(s => s.Get(It.IsAny<Guid>())).ReturnsAsync(new ResponseCode()
        {
            Id = new Guid(),
            Machine = Machine.Deposit,
            State = "Success",
            Suggestion = "Success",
            Description = "Success"
        });
        _responseCodeRepository.Setup(s => s.GetByStates(It.IsAny<Machine>(), It.IsAny<string[]>())).ReturnsAsync(new List<ResponseCode>
        {
            new()
            {
                Id = new Guid(),
                Machine = Machine.Deposit,
                State = "Success",
                Suggestion = "Success",
                Description = "Success"
            }
        });

        var transferCashPaginateRequest = new TransferCashPaginateRequest(null, null,
            new TransferCashFilterRequest(
                status, null, null, null, null, null, null, null, null, null, null), 1, 20);
        var filters = new TransferCashFilter(status, null, null, null, null, null, null, null, null, null, null);

        var actual = await _query.GetTransferCashPaginate(transferCashPaginateRequest.Page, transferCashPaginateRequest.PageSize, null, null, filters);

        Assert.NotNull(actual);
        Assert.Equal(expected.Data.FirstOrDefault()?.State, actual.Records[0].Transaction.State);
        Assert.Equal(expected.Data.FirstOrDefault()?.TransactionNo, actual.Records[0].Transaction.TransactionNo);
        Assert.Equal(expected.Data.FirstOrDefault()?.Status, actual.Records[0].Transaction.Status);
        Assert.Equal(expected.Data.FirstOrDefault()?.CustomerName, actual.Records[0].Transaction.CustomerName);
        Assert.Equal(expected.Data.FirstOrDefault()?.TransferFromAccountCode, actual.Records[0].Transaction.TransferFromAccountCode);
        Assert.Equal(expected.Data.FirstOrDefault()?.TransferToAccountCode, actual.Records[0].Transaction.TransferToAccountCode);
        Assert.Equal(expected.Data.FirstOrDefault()?.TransferFromExchangeMarket, actual.Records[0].Transaction.TransferFromExchangeMarket);
        Assert.Equal(expected.Data.FirstOrDefault()?.TransferToExchangeMarket, actual.Records[0].Transaction.TransferToExchangeMarket);
        Assert.Equal(expected.Data.FirstOrDefault()?.Amount, actual.Records[0].Transaction.Amount);
        Assert.Equal(expected.Data.FirstOrDefault()?.FailedReason, actual.Records[0].Transaction.FailedReason);
        Assert.Equal(expected.Data.FirstOrDefault()?.OtpConfirmedDateTime, actual.Records[0].Transaction.OtpConfirmedDateTime);
        Assert.Equal(expected.Data.FirstOrDefault()?.CreatedAt, actual.Records[0].Transaction.CreatedAt);
    }

    private void SetupGetTransactionV2(PaginateResponse<TransactionHistoryV2> expected)
    {
        _depositWithdrawService.Setup(s =>
                s.GetTransactionHistoriesV2(It.IsAny<TransactionHistoryV2Filter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);
        _responseCodeRepository.Setup(s => s.Get(It.IsAny<Guid>())).ReturnsAsync(new ResponseCode()
        {
            Id = new Guid(),
            Machine = Machine.Deposit,
            State = "Success",
            Suggestion = "Success",
            Description = "Success"
        });
        _responseCodeRepository.Setup(s => s.GetByStates(It.IsAny<Machine>(), It.IsAny<string[]>())).ReturnsAsync(new List<ResponseCode>
        {
            new()
            {
                Id = new Guid(),
                Machine = Machine.Deposit,
                State = "Success",
                Suggestion = "Success",
                Description = "Success"
            }
        });
    }

    private void SetupGetTransactionV2(TransactionV2 expected)
    {
        _depositWithdrawService.Setup(s =>
                s.GetTransactionV2ByTransactionNo(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);
        _responseCodeRepository.Setup(s => s.GetByStateMachine(It.IsAny<Machine>(), It.IsAny<string>(), It.IsAny<ProductType?>()))
            .ReturnsAsync(new ResponseCode
            {
                Id = new Guid(),
                Machine = Machine.Deposit,
                State = "Success",
                Suggestion = "Success",
                Description = "Success"
            });
        _responseCodeActionRepository.Setup(s => s.GetByGuid(It.IsAny<Guid>()))
            .ReturnsAsync(new List<ResponseCodeAction>());
    }

    private static TransactionV2 GetExpectedResponse(string transactionNo)
    {
        var fixedDateTime = new DateTime(2025, 01, 01);
        return new TransactionV2
        {
            Id = Guid.NewGuid(),
            CustomerCode = "CUST123",
            FailedReason = "None",
            PaymentReceivedAmount = "20.00",
            PaymentDisbursedAmount = "20.00",
            ConfirmedAmount = "20.00",
            TransactionNo = transactionNo,
            Channel = Channel.QR,
            Product = Product.Cash,
            TransactionType = TransactionType.Deposit,
            QrDeposit = new QrDepositState
            {
                CreatedAt = fixedDateTime,
                FailedReason = "None",
                DepositQrGenerateDateTime = fixedDateTime,
                Fee = 0,
                PaymentReceivedAmount = 20,
                PaymentReceivedDateTime = fixedDateTime,
                QrTransactionNo = "QrTransactionNo",
                QrTransactionRef = "QrTransactionStatus",
                QrValue = "QrValue",
                State = "Final"
            },
            OddDeposit = new OddDepositState
            {
                State = "Completed",
                PaymentReceivedAmount = 50,
                PaymentReceivedDateTime = fixedDateTime,
                Fee = 1,
                CreatedAt = fixedDateTime,
                UpdatedAt = fixedDateTime
            },
            AtsDeposit = new AtsDepositState
            {
                State = "Completed",
                PaymentReceivedAmount = 100,
                PaymentReceivedDateTime = fixedDateTime,
                Fee = 2,
                CreatedAt = fixedDateTime,
                UpdatedAt = fixedDateTime
            },
            OddWithdraw = new OddWithdrawState
            {
                State = "Completed",
                PaymentDisbursedAmount = 30,
                PaymentDisbursedDateTime = fixedDateTime,
                Fee = 1,
                CreatedAt = fixedDateTime,
                UpdatedAt = fixedDateTime
            },
            AtsWithdraw = new AtsWithdrawState
            {
                State = "Completed",
                PaymentDisbursedAmount = 40,
                PaymentDisbursedDateTime = fixedDateTime,
                Fee = 2,
                CreatedAt = fixedDateTime,
                UpdatedAt = fixedDateTime
            },
            GlobalTransfer = new GlobalTransferState
            {
                State = "Completed",
                GlobalAccount = "GlobalAccount123",
                RequestedCurrency = Currency.USD,
                RequestedFxRate = 1.2m,
                RequestedFxCurrency = Currency.THB,
                PaymentReceivedAmount = 200,
                PaymentReceivedCurrency = Currency.USD,
                FxInitiateRequestDateTime = fixedDateTime,
                FxTransactionId = "FxTransactionId123",
                FxConfirmedAmount = 180,
                FxConfirmedExchangeRate = 1.1m,
                FxConfirmedCurrency = Currency.THB,
                FxConfirmedDateTime = fixedDateTime,
                TransferAmount = 150,
                TransferCurrency = Currency.USD,
                TransferFee = 5,
                TransferFromAccount = "AccountFrom123",
                TransferToAccount = "AccountTo123",
                TransferRequestTime = fixedDateTime,
                TransferCompleteTime = fixedDateTime,
                FailedReason = "None",
                CreatedAt = fixedDateTime,
                UpdatedAt = fixedDateTime
            },
            UpBack = new UpBackState
            {
                State = "Completed",
                FailedReason = "None",
                CreatedAt = fixedDateTime,
                UpdatedAt = fixedDateTime
            },
            Recovery = new RecoveryState
            {
                State = "Completed",
                GlobalAccount = "GlobalAccount123",
                TransferFromAccount = "AccountFrom123",
                TransferAmount = 100,
                TransferToAccount = "AccountTo123",
                TransferCurrency = Currency.USD,
                TransferRequestTime = fixedDateTime,
                TransferCompleteTime = fixedDateTime,
                FailedReason = "None",
                CreatedAt = fixedDateTime,
                UpdatedAt = fixedDateTime
            },
            Refund = new RefundInfo
            {
                RefundId = "RefundId123",
                Amount = 50,
                TransferToAccountNo = "AccountNo123",
                TransferToAccountName = "AccountName123",
                Fee = 1,
                CreatedAt = fixedDateTime
            },
            BillPayment = new BillPaymentState
            {
                State = "Completed",
                PaymentReceivedAmount = 100,
                PaymentReceivedDateTime = fixedDateTime,
                Reference1 = "AccountCode123",
                Reference2 = "TaxId123",
                CustomerPaymentName = "CustomerName123",
                CustomerPaymentBankCode = "BankCode123",
                Fee = 2,
                BillPaymentTransactionRef = "TransactionRef123",
                FailedReason = "None"
            }
        };
    }

    private static PaginateResponse<TransactionHistoryV2> GetExpectedPaginateResponse()
    {
        return new PaginateResponse<TransactionHistoryV2>(
            new List<TransactionHistoryV2> { new()
            {
                State = "Final",
                Product = Product.Cash,
                AccountCode = "123456",
                CustomerName = "John Doe",
                BankAccountNo = "11223344567",
                BankAccountName = "John Doe",
                BankName = "The Siam Commercial Bank Public Company Limited",
                EffectiveDate = DateOnly.FromDateTime(DateTime.Now),
                GlobalAccount = null,
                TransactionNo = "DHDP2000010100001",
                TransactionType = TransactionType.Deposit,
                RequestedAmount = null,
                RequestedCurrency = Currency.THB,
                Status = "Success",
                CreatedAt = DateTime.Now,
                ToCurrency = null,
                TransferAmount = "20.00",
                Channel = Channel.QR,
                BankAccount = "The Siam Commercial Bank Public Company Limited • 0099",
                Fee = "null",
                TransferFee = "null",
            } }, 1, 20, 100, null, null);
    }

    private static PaginateResponse<TransferCash> GetExpectedTransferCashPaginateResponse()
    {
        return new PaginateResponse<TransferCash>(
        [
            new TransferCash
            {
                State = "Final",
                TransactionNo = "TFC2000010100001",
                Status = "Success",
                CustomerName = "John Doe",
                TransferFromAccountCode = "12345",
                TransferToAccountCode = "67890",
                TransferFromExchangeMarket = Product.Cash,
                TransferToExchangeMarket = Product.CashBalance,
                Amount = 100,
                FailedReason = "",
                OtpConfirmedDateTime = DateTime.Now,
                CreatedAt = DateTime.Now
            }
        ], 1, 20, 100, null, null);
    }
}