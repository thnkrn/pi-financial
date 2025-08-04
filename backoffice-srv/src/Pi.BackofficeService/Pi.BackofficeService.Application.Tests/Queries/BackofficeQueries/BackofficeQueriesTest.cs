using Moq;
using Pi.BackofficeService.Application.Queries;
using Pi.BackofficeService.Application.Queries.Filters;
using Pi.BackofficeService.Application.Services.UserService;
using Pi.BackofficeService.Application.Services.WalletService;
using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransferCashAggregate;
using Pi.BackofficeService.Domain.SeedWork;

namespace Pi.BackofficeService.Application.Tests.Queries.BackofficeQueries;

public class BackofficeQueriesTest
{
    private readonly Mock<IResponseCodeActionRepository> _responseCodeActionRepository;
    private readonly Mock<IResponseCodeRepository> _responseCodeRepository;
    private readonly Mock<ITransactionRepository> _transactionRepository;
    private readonly Mock<IDepositWithdrawService> _depositWithdrawService;
    private readonly Mock<ITransferCashService> _transferCashService;
    private readonly Application.Queries.BackofficeQueries _query;

    public BackofficeQueriesTest()
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
    [InlineData(ProductType.GlobalEquity, Product.GlobalEquity)]
    [InlineData(ProductType.ThaiEquity, Product.Cash)]
    [InlineData(null, Product.Crypto)]
    public async Task Should_Return_Products_When_GetProducts(ProductType? productType, Product expected)
    {
        // Act
        var actual = await _query.GetProducts(productType);

        // Assert
        Assert.Contains(expected, actual);
    }

    [Fact]
    public async Task Should_Return_ApplicationDepositChannels_When_GetDepositChannels()
    {
        // Act
        var actual = await _query.GetDepositChannels();

        // Assert
        Assert.IsType<List<Models.DepositChannel>>(actual);
    }

    [Fact]
    public async Task Should_Return_ApplicationWithdrawChannels_When_GetWithdrawChannels()
    {
        // Act
        var actual = await _query.GetWithdrawChannels();

        // Assert
        Assert.IsType<List<Models.WithdrawChannel>>(actual);
    }

    [Fact]
    public async Task Should_Return_ExpectedBanks_When_GetBanks_WithoutChannel()
    {
        // Arrange
        var bank = new Bank("001", "BankName", "KBANK");
        _transactionRepository.Setup(q => q.GetBanks()).ReturnsAsync(new List<Bank>() { bank });

        // Act
        var actual = await _query.GetBanks(null);

        // Assert
        Assert.IsType<List<Bank>>(actual);
        Assert.Equal(bank.Code, actual.First().Code);
    }

    [Fact]
    public async Task Should_Return_ExpectedBanks_When_GetBanks_WithChannel()
    {
        // Arrange
        var channel = DepositChannel.AtsBatch.ToString();
        var bank = new Bank("001", "BankName", "KBANK");
        _transactionRepository.Setup(q => q.GetBanksByChannelAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<Bank>() { bank });

        // Act
        var actual = await _query.GetBanks(channel);

        // Assert
        Assert.IsType<List<Bank>>(actual);
        Assert.Equal(bank.Code, actual.First().Code);
    }

    [Fact]
    public async Task Should_Return_ResponseCodes_When_GetResponseCodes()
    {
        // Arrange
        var error = new ResponseCode() { Id = Guid.NewGuid() };
        var filter = new ResponseCodeFilter();
        _responseCodeRepository.Setup(q => q.Get(It.IsAny<IQueryFilter<ResponseCode>>())).ReturnsAsync(new List<ResponseCode>() { error });

        // Act
        var actual = await _query.GetResponseCodes(filter);

        // Assert
        Assert.IsType<List<ResponseCode>>(actual);
    }

    [Fact]
    public async Task Should_Return_ExpectedResponseCodes_When_GetResponseCodes()
    {
        // Arrange
        var error = new ResponseCode() { Id = Guid.NewGuid() };
        var filter = new ResponseCodeFilter();
        _responseCodeRepository.Setup(q => q.Get(It.IsAny<IQueryFilter<ResponseCode>>())).ReturnsAsync(new List<ResponseCode>() { error });

        // Act
        var actual = await _query.GetResponseCodes(filter);

        // Assert
        Assert.Equal(error.Id, actual.First().Id);
    }

    [Fact]
    public async Task Should_Return_ResponseCodeAction_When_GetResponseCodeAction()
    {
        // Arrange
        var responseCodeAction = new ResponseCodeAction(Guid.NewGuid(), Guid.NewGuid(), Method.Approve);
        _responseCodeActionRepository.Setup(q => q.GetByGuid(It.IsAny<Guid>())).ReturnsAsync(new List<ResponseCodeAction>() { responseCodeAction });

        // Act
        var actual = await _query.GetResponseCodeAction(Guid.NewGuid());

        // Assert
        Assert.IsType<List<ResponseCodeAction>>(actual);
    }

    [Fact]
    public async Task Should_Return_ExpectedResponseCodeAction_When_GetResponseCodeAction()
    {
        // Arrange
        var responseCodeAction = new ResponseCodeAction(Guid.NewGuid(), Guid.NewGuid(), Method.Approve);
        _responseCodeActionRepository.Setup(q => q.GetByGuid(It.IsAny<Guid>())).ReturnsAsync(new List<ResponseCodeAction>() { responseCodeAction });

        // Act
        var actual = await _query.GetResponseCodeAction(Guid.NewGuid());

        // Assert
        Assert.Equal(responseCodeAction.Id, actual.First().Id);
    }

    [Fact]
    public async Task GetTransactionV2ByTransactionNo_ShouldReturnCorrectResponse()
    {
        // Arrange
        var transactionNo = "transactionNo";
        var transactionV2 = new TransactionV2
        {
            State = "currentState",
            Status = "Pending",
            TransactionNo = transactionNo,
            Product = Product.Cash,
            Channel = Channel.QR,
            TransactionType = TransactionType.Deposit,
        };
        var transactionDetailResult = new TransactionDetailResult<TransactionV2>
        {
            Transaction = transactionV2,
            ResponseCodeDetail = null // set this as needed
        };
        _depositWithdrawService.Setup(service => service.GetTransactionV2ByTransactionNo(transactionNo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactionDetailResult.Transaction);

        // Act
        var actual = await _query.GetTransactionV2ByTransactionNo(transactionNo);

        // Assert
        Assert.NotNull(actual);
        Assert.IsType<TransactionDetailResult<TransactionV2>>(actual);
        Assert.Equal(transactionV2.State, actual.Transaction.State);
        Assert.Equal(transactionV2.Status, actual.Transaction.Status);
        Assert.Equal(transactionV2.TransactionNo, actual.Transaction.TransactionNo);
    }

    [Fact]
    public async Task GetTransactionV2ByTransactionNo_ShouldReturnResponseCode()
    {
        // Arrange
        var transactionNo = "transactionNo";
        var transactionV2 = new TransactionV2
        {
            State = "currentState",
            Status = "Pending",
            TransactionNo = transactionNo,
            Product = Product.Cash,
            Channel = Channel.QR,
            TransactionType = TransactionType.Withdraw,
        };
        var transactionDetailResult = new TransactionDetailResult<TransactionV2>
        {
            Transaction = transactionV2,
            ResponseCodeDetail = null // set this as needed
        };
        _depositWithdrawService.Setup(service => service.GetTransactionV2ByTransactionNo(transactionNo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactionDetailResult.Transaction);

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
            .ReturnsAsync(new List<ResponseCodeAction>
            {
                new(new Guid(), new Guid(), Method.Approve),
            });

        // Act
        var actual = await _query.GetTransactionV2ByTransactionNo(transactionNo);

        // Assert
        Assert.NotNull(actual);
        Assert.IsType<TransactionDetailResult<TransactionV2>>(actual);
        Assert.Equal(transactionV2.State, actual.Transaction.State);
        Assert.Equal(transactionV2.Status, actual.Transaction.Status);
        Assert.Equal(transactionV2.TransactionNo, actual.Transaction.TransactionNo);
    }

    [Fact]
    public async Task GetTransferCashByTransactionNo_ShouldReturnResponseCode()
    {
        // Arrange
        var transactionNo = "transactionNo";
        var transferCash = new TransferCash
        {
            State = "currentState",
            Status = "Pending",
            TransactionNo = transactionNo,
        };
        var transactionDetailResult = new TransactionDetailResult<TransferCash>
        {
            Transaction = transferCash,
            ResponseCodeDetail = null // set this as needed
        };
        _transferCashService.Setup(service => service.GetTransferCashByTransactionNo(transactionNo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactionDetailResult.Transaction);

        _responseCodeRepository.Setup(s => s.GetByStateMachine(It.IsAny<Machine>(), It.IsAny<string>(), It.IsAny<ProductType?>()))
            .ReturnsAsync(new ResponseCode
            {
                Id = new Guid(),
                Machine = Machine.TransferCash,
                State = "Success",
                Suggestion = "Success",
                Description = "Success"
            });

        _responseCodeActionRepository.Setup(s => s.GetByGuid(It.IsAny<Guid>()))
            .ReturnsAsync(new List<ResponseCodeAction>
            {
                new(new Guid(), new Guid(), Method.Approve),
            });

        // Act
        var actual = await _query.GetTransferCashByTransactionNo(transactionNo, new CancellationToken());

        // Assert
        Assert.NotNull(actual);
        Assert.IsType<TransactionDetailResult<TransferCash>>(actual);
        Assert.Equal(transferCash.State, actual.Transaction.State);
        Assert.Equal(transferCash.Status, actual.Transaction.Status);
        Assert.Equal(transferCash.TransactionNo, actual.Transaction.TransactionNo);
    }
}
