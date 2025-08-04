using Moq;
using Pi.Common.Features;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Application.Services;
using Pi.WalletService.Application.Services.UserService;
using Pi.WalletService.Domain;
using Pi.WalletService.Domain.AggregatesModel.CashAggregate;
using Pi.WalletService.Domain.AggregatesModel.DepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.RefundAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Tests.Queries;

public class TransactionQueriesTest
{
    private readonly Mock<IDepositRepository> _depositRepositoryMock;
    private readonly Mock<IGlobalWalletDepositRepository> _globalWalletDepositRepositoryMock;
    private readonly ITransactionQueries _transactionQueries;
    private readonly Mock<IWithdrawRepository> _withdrawRepositoryMock;
    private readonly Mock<IRefundRepository> _refundRepository;
    private readonly Mock<IDepositEntrypointRepository> _depositEntrypointRepositoryMock;
    private readonly Mock<IWithdrawEntrypointRepository> _withdrawEntrypointRepositoryMock;


    public TransactionQueriesTest()
    {
        Mock<IFeatureService> featureServiceMock = new();
        _depositRepositoryMock = new Mock<IDepositRepository>();
        _globalWalletDepositRepositoryMock = new Mock<IGlobalWalletDepositRepository>();
        _withdrawRepositoryMock = new Mock<IWithdrawRepository>();
        Mock<ICashDepositRepository> cashDepositRepositoryMock = new();
        _depositEntrypointRepositoryMock = new Mock<IDepositEntrypointRepository>();
        _withdrawEntrypointRepositoryMock = new Mock<IWithdrawEntrypointRepository>();
        _refundRepository = new Mock<IRefundRepository>();
        Mock<ICashWithdrawRepository> cashWithdrawRepositoryMock = new();
        Mock<ITransactionHistoryService> transactionHistoryServiceMock = new();
        Mock<IUserService> userServiceMock = new();

        _transactionQueries = new TransactionQueries(
            featureServiceMock.Object,
            _depositEntrypointRepositoryMock.Object,
            _withdrawEntrypointRepositoryMock.Object,
            _depositRepositoryMock.Object,
            _globalWalletDepositRepositoryMock.Object,
            _withdrawRepositoryMock.Object,
            cashDepositRepositoryMock.Object,
            _refundRepository.Object,
            cashWithdrawRepositoryMock.Object,
            transactionHistoryServiceMock.Object
            );
    }

    [Fact]
    public async Task TransactionQueries_GetDepositTransactions_ShouldReturnDepositTransactions()
    {
        // Arrange
        _depositRepositoryMock.Setup(q => q.Get(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<DepositStateFilters>()
        )).ReturnsAsync(new List<DepositState> { new() });

        // Act
        var transactions = await _transactionQueries.GetDepositTransactions(new PaginateRequest(1, 10, null, null), null);

        // Assert
        Assert.IsType<DepositTransaction>(transactions.First());
    }

    [Fact]
    public async Task TransactionQueries_GetDepositTransactions_ShouldReturnDepositTransactions_When_FilterByProductTypeGE()
    {
        // Arrange
        var deposit = new DepositState()
        {
            CurrentState = IntegrationEvents.Models.DepositState.GetName(() =>
                    IntegrationEvents.Models.DepositState.DepositCompleted),
        };
        var filters = new TransactionFilters(null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, ProductType.GlobalEquity);
        _depositRepositoryMock.Setup(q => q.Get(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<IQueryFilter<DepositState>>()
        )).ReturnsAsync(new List<DepositState> { deposit });

        // Act
        var transactions = await _transactionQueries.GetDepositTransactions(new PaginateRequest(1, 10, null, null), filters);

        // Assert
        Assert.IsType<DepositTransaction>(transactions.First());
    }

    [Fact]
    public async Task TransactionQueries_GetDepositTransactionByTransactionNo_ShouldReturnDepositTransaction()
    {
        // Arrange
        var transactionNo = "someTransactionNo";
        _depositRepositoryMock.Setup(q => q.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(new DepositState()
            {
                CurrentState = IntegrationEvents.Models.DepositState.GetName(
                    () => IntegrationEvents.Models.DepositState.DepositCompleted)
            });

        // Act
        var transaction = await _transactionQueries.GetDepositTransactionByTransactionNo<DepositTransaction>(transactionNo);

        // Assert
        Assert.IsType<DepositTransaction>(transaction);
    }

    [Fact]
    public async Task TransactionQueries_GetDepositTransactionByTransactionNo_ShouldReturnGlobalTransaction()
    {
        // Arrange
        var transactionNo = "someTransactionNo";
        _depositRepositoryMock.Setup(q => q.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(new DepositState()
            {
                CurrentState = IntegrationEvents.Models.DepositState.GetName(() => IntegrationEvents.Models.DepositState.DepositCompleted),
                Product = Product.GlobalEquities
            });
        _globalWalletDepositRepositoryMock.Setup(q => q.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(new GlobalWalletTransferState()
            {
                FxConfirmedAmount = 123,
                TransferCurrency = Currency.THB,
                RequestedCurrency = Currency.USD,
                RequestedAmount = 1000,
                FxConfirmedExchangeRate = (decimal?)1.5
            });

        // Act
        var transaction = await _transactionQueries.GetDepositTransactionByTransactionNo<GlobalTransaction>(transactionNo);

        // Assert
        Assert.IsType<GlobalTransaction>(transaction);
    }

    [Fact]
    public async Task TransactionQueries_GetDepositTransactionByTransactionNo_ShouldReturnNull_When_TransactionNotFound()
    {
        // Arrange
        var transactionNo = "someTransactionNo";

        // Act
        var transaction = await _transactionQueries.GetDepositTransactionByTransactionNo<DepositTransaction>(transactionNo);

        // Assert
        Assert.Null(transaction);
    }

    [Fact]
    public async Task TransactionQueries_GetDepositTransactionByTransactionNo_ShouldReturnNull_When_GlobalTransferTransactionNotFound()
    {
        // Arrange
        var transactionNo = "someTransactionNo";
        _depositRepositoryMock.Setup(q => q.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(new DepositState()
            {
                CurrentState = IntegrationEvents.Models.DepositState.GetName(() => IntegrationEvents.Models.DepositState.DepositCompleted),
                Product = Product.GlobalEquities
            });

        // Act
        var transaction = await _transactionQueries.GetDepositTransactionByTransactionNo<GlobalTransaction>(transactionNo);

        // Assert
        Assert.Null(transaction);
    }

    [Fact]
    public async Task TransactionQueries_GetWithdrawTransactions_ShouldReturnWithdrawTransactions()
    {
        // Arrange
        _withdrawRepositoryMock.Setup(q => q.Get(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<WithdrawStateFilters>()
        )).ReturnsAsync(new List<WithdrawState> { new() });

        // Act
        var transactions = await _transactionQueries.GetWithdrawTransactions(new PaginateRequest(1, 10, null, null), null);

        // Assert
        Assert.IsType<WithdrawTransaction>(transactions.First());
    }

    [Fact]
    public async Task TransactionQueries_GetWithdrawTransactions_ShouldReturnWithdrawTransactions_With_GlobalState_When_FilterByProductTypeGE()
    {
        // Arrange
        var withdrawState = new WithdrawState()
        {
            CurrentState = "Final",
        };
        var filters = new TransactionFilters(null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, ProductType.GlobalEquity);
        _withdrawRepositoryMock.Setup(q => q.Get(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<IQueryFilter<WithdrawState>>()
        )).ReturnsAsync(new List<WithdrawState> { withdrawState });

        // Act
        var transactions = await _transactionQueries.GetWithdrawTransactions(new PaginateRequest(1, 10, null, null), filters);

        // Assert
        Assert.IsType<WithdrawTransaction>(transactions.First());
    }

    [Fact]
    public async Task TransactionQueries_GetWithdrawTransactionByTransactionNo_ShouldReturnWithdrawTransaction()
    {
        // Arrange
        var transactionNo = "someTransactionNo";
        _withdrawRepositoryMock.Setup(q => q.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(new WithdrawState()
            {
                CurrentState = IntegrationEvents.Models.WithdrawState.GetName(
                    () => IntegrationEvents.Models.WithdrawState.WithdrawalProcessing)
            });

        // Acts
        var transaction = await _transactionQueries.GetWithdrawTransactionByTransactionNo<WithdrawTransaction>(transactionNo);

        // Assert
        Assert.IsType<WithdrawTransaction>(transaction);
    }

    [Fact]
    public async Task TransactionQueries_GetWithdrawTransactionByTransactionNo_ShouldReturnGlobalTransaction()
    {
        // Arrange
        var transactionNo = "someTransactionNo";
        _withdrawRepositoryMock.Setup(q => q.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(new WithdrawState()
            {
                CurrentState = "Final",
                Product = Product.GlobalEquities
            });
        _globalWalletDepositRepositoryMock.Setup(q => q.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(new GlobalWalletTransferState()
            {
                FxConfirmedAmount = 123,
                TransferCurrency = Currency.THB,
                RequestedCurrency = Currency.USD,
                RequestedAmount = 1000,
                FxConfirmedExchangeRate = (decimal?)1.5
            });

        // Act
        var transaction = await _transactionQueries.GetWithdrawTransactionByTransactionNo<GlobalTransaction>(transactionNo);

        // Assert
        Assert.IsType<GlobalTransaction>(transaction);
    }

    [Fact]
    public async Task TransactionQueries_GetWithdrawTransactionByTransactionNo_ShouldReturnNull_When_TransactionNotFound()
    {
        // Arrange
        var transactionNo = "someTransactionNo";

        // Act
        var transaction = await _transactionQueries.GetWithdrawTransactionByTransactionNo<WithdrawTransaction>(transactionNo);

        // Assert
        Assert.Null(transaction);
    }

    [Fact]
    public async Task TransactionQueries_GetWithdrawTransactionByTransactionNo_ShouldReturnNull_When_GlobalTransferTransactionNotFound()
    {
        // Arrange
        var transactionNo = "someTransactionNo";
        _withdrawRepositoryMock.Setup(q => q.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(new WithdrawState()
            {
                CurrentState = "Final",
                Product = Product.GlobalEquities
            });

        // Act
        var transaction = await _transactionQueries.GetWithdrawTransactionByTransactionNo<GlobalTransaction>(transactionNo);

        // Assert
        Assert.Null(transaction);
    }

    [Fact]
    public async Task TransactionQueries_GetRefundTransactions_ShouldReturnRefundTransactions()
    {
        // Arrange
        _refundRepository.Setup(q => q.Get(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<RefundStateFilters>()
        )).ReturnsAsync(new List<RefundState> { new() { RefundedAt = DateTime.Now } });

        // Act
        var transactions = await _transactionQueries.GetRefundTransactions(new PaginateRequest(1, 10, null, null), null);

        // Assert
        Assert.IsType<RefundTransaction>(transactions.First());
    }

    [Fact]
    public async Task TransactionQueries_GetRefundTransactions_ShouldReturnRefundTransactions_When_FilterNotNull()
    {
        // Arrange
        var filters = new TransactionFilters(null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, ProductType.GlobalEquity);
        _refundRepository.Setup(q => q.Get(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<RefundStateFilters>()
        )).ReturnsAsync(new List<RefundState> { new() { RefundedAt = DateTime.Now } });

        // Act
        var transactions = await _transactionQueries.GetRefundTransactions(new PaginateRequest(1, 10, null, null), filters);

        // Assert
        Assert.IsType<RefundTransaction>(transactions.First());
    }

    [Fact]
    public async Task TransactionQueries_GetRefundTransactions_ShouldReturnExpectedRefundTransactions()
    {
        // Arrange
        var expected = new List<RefundState> { new() { RefundedAt = DateTime.Now } };
        _refundRepository.Setup(q => q.Get(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<RefundStateFilters>()
        )).ReturnsAsync(expected);

        // Act
        var transactions = await _transactionQueries.GetRefundTransactions(new PaginateRequest(1, 10, null, null), null);

        // Assert
        Assert.Equal(expected.First().RefundedAt, transactions.First().RefundedAt);
    }

    [Theory]
    [InlineData(TransactionType.Deposit, 1)]
    [InlineData(TransactionType.Withdraw, 1)]
    [InlineData(null, 2)]
    public async Task TransactionQueries_CountTransactions_ShouldReturnExpected_When_FilterByTransactionType(
        TransactionType? transactionType, int expected)
    {
        // Arrange
        var filters = new TransactionFilters(null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            transactionType);
        _depositRepositoryMock.Setup(q => q.CountTransactions(It.IsAny<DepositStateFilters>()))
            .ReturnsAsync(1);
        _withdrawRepositoryMock.Setup(q => q.CountTransactions(It.IsAny<WithdrawStateFilters>()))
            .ReturnsAsync(1);

        // Act
        var actual = await _transactionQueries.CountTransactions(filters);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(ProductType.ThaiEquity, 1)]
    [InlineData(ProductType.GlobalEquity, 1)]
    public async Task TransactionQueries_CountDepositTransactions_ShouldReturnExpected_When_FilterByProduct(
        ProductType? productType, int expected)
    {
        // Arrange
        var filters = new TransactionFilters(null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, productType);
        _depositRepositoryMock.Setup(q => q.CountTransactions(It.IsAny<IQueryFilter<DepositState>>()))
            .ReturnsAsync(1);

        // Act
        var actual = await _transactionQueries.CountDepositTransactions(filters);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(ProductType.ThaiEquity, 1)]
    [InlineData(ProductType.GlobalEquity, 1)]
    public async Task TransactionQueries_CountWithdrawTransactions_ShouldReturnExpected_When_FilterByProduct(
        ProductType? productType, int expected)
    {
        // Arrange
        var filters = new TransactionFilters(null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, productType);
        _withdrawRepositoryMock.Setup(q => q.CountTransactions(It.IsAny<IQueryFilter<WithdrawState>>()))
            .ReturnsAsync(1);

        // Act
        var actual = await _transactionQueries.CountWithdrawTransactions(filters);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task TransactionQueries_CountRefundTransactions_ShouldReturnExpected()
    {
        // Arrange
        var expected = 1;
        var filters = new TransactionFilters(null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null);
        _refundRepository.Setup(q => q.CountTransactions(It.IsAny<IQueryFilter<RefundState>>()))
            .ReturnsAsync(1);

        // Act
        var actual = await _transactionQueries.CountRefundTransactions(filters);

        // Assert
        Assert.Equal(expected, actual);
    }
}
