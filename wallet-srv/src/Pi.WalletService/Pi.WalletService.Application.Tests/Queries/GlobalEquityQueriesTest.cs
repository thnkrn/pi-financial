using Moq;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Application.Queries.Filters;
using Pi.WalletService.Domain;
using Pi.WalletService.Domain.AggregatesModel.DepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawAggregate;

namespace Pi.WalletService.Application.Tests.Queries;

public class GlobalEquityQueriesTest
{
    private readonly Mock<IDepositRepository> _depositRepositoryMock;
    private readonly Mock<IGlobalWalletDepositRepository> _globalWalletDepositRepositoryMock;
    private readonly Mock<IWithdrawRepository> _withdrawRepositoryMock;
    private readonly GlobalEquityQueries _globalEquityQueries;

    public GlobalEquityQueriesTest()
    {
        _depositRepositoryMock = new Mock<IDepositRepository>();
        _globalWalletDepositRepositoryMock = new Mock<IGlobalWalletDepositRepository>();
        _withdrawRepositoryMock = new Mock<IWithdrawRepository>();
        _globalEquityQueries = new GlobalEquityQueries(
            _depositRepositoryMock.Object,
            _withdrawRepositoryMock.Object,
            _globalWalletDepositRepositoryMock.Object);
    }

    [Fact]
    public async Task GlobalEquityQueries_GetDepositTransactions_ShouldReturnExpectedDepositTransactions()
    {
        // Arrange
        var deposit = new DepositState()
        {
            CorrelationId = Guid.NewGuid(),
            CurrentState = IntegrationEvents.Models.DepositState.GetName(() =>
                IntegrationEvents.Models.DepositState.DepositCompleted),
        };
        var filters = new TransactionFilters(null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, ProductType.GlobalEquity);
        _depositRepositoryMock.Setup(q => q.GetGlobalDeposit(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<GlobalDepositFilters>()
        )).ReturnsAsync(new List<GlobalDepositTransaction>
            {
                new()
                {
                    DepositState = deposit,
                    GlobalWalletTransferState = new GlobalWalletTransferState()
                    {
                        CurrentState = IntegrationEvents.Models.GlobalWalletTransferState.GetName(() =>
                            IntegrationEvents.Models.GlobalWalletTransferState.FxFailed)
                    }
                }
            }
        );

        // Act
        var transactions = await _globalEquityQueries.GetDepositTransactions(new PaginateRequest(1, 10, null, null), filters);

        // Assert
        Assert.IsType<DepositTransaction>(transactions.First());
        Assert.Equal(deposit.CorrelationId, transactions.First().Id);
    }

    [Fact]
    public async Task GlobalEquityQueries_GetDepositTransactions_Without_Filter_ShouldReturnExpectedDepositTransactions()
    {
        // Arrange
        var deposit = new DepositState()
        {
            CorrelationId = Guid.NewGuid(),
            CurrentState = IntegrationEvents.Models.DepositState.GetName(() =>
                IntegrationEvents.Models.DepositState.DepositCompleted),
        };
        _depositRepositoryMock.Setup(q => q.GetGlobalDeposit(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<GlobalDepositFilters>()
        )).ReturnsAsync(new List<GlobalDepositTransaction>
            {
                new()
                {
                    DepositState = deposit,
                    GlobalWalletTransferState = new GlobalWalletTransferState()
                    {
                        CurrentState = IntegrationEvents.Models.GlobalWalletTransferState.GetName(() =>
                            IntegrationEvents.Models.GlobalWalletTransferState.FxFailed)
                    }
                }
            }
        );

        // Act
        var transactions = await _globalEquityQueries.GetDepositTransactions(new PaginateRequest(1, 10, null, null), null);

        // Assert
        Assert.IsType<DepositTransaction>(transactions.First());
        Assert.Equal(deposit.CorrelationId, transactions.First().Id);
    }

    [Fact]
    public async Task GlobalEquityQueries_GetWithdrawTransactions_ShouldReturnExpectedWithdrawTransactions()
    {
        // Arrange
        var withdrawState = new WithdrawState()
        {
            CorrelationId = Guid.NewGuid(),
            CurrentState = "Final",
        };
        var filters = new TransactionFilters(null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, ProductType.GlobalEquity);
        _withdrawRepositoryMock.Setup(q => q.GetGlobalWithdraw(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<IQueryFilter<GlobalWithdrawTransaction>>()
        )).ReturnsAsync(new List<GlobalWithdrawTransaction> { new()
        {
            WithdrawState = withdrawState,
            GlobalWalletTransferState = new GlobalWalletTransferState()
            {
                CurrentState = IntegrationEvents.Models.GlobalWalletTransferState.GetName(() => IntegrationEvents.Models.GlobalWalletTransferState.FxFailed)
            }
        } });

        // Act
        var transactions = await _globalEquityQueries.GetWithdrawTransactions(new PaginateRequest(1, 10, null, null), filters);

        // Assert
        Assert.IsType<WithdrawTransaction>(transactions.First());
        Assert.Equal(withdrawState.CorrelationId, transactions.First().Id);
    }

    [Fact]
    public async Task GlobalEquityQueries_GetWithdrawTransactions_Without_Filter_ShouldReturnExpectedWithdrawTransactions()
    {
        // Arrange
        var withdrawState = new WithdrawState()
        {
            CorrelationId = Guid.NewGuid(),
            CurrentState = "Final",
        };
        _withdrawRepositoryMock.Setup(q => q.GetGlobalWithdraw(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<IQueryFilter<GlobalWithdrawTransaction>>()
        )).ReturnsAsync(new List<GlobalWithdrawTransaction> { new()
        {
            WithdrawState = withdrawState,
            GlobalWalletTransferState = new GlobalWalletTransferState()
            {
                CurrentState = IntegrationEvents.Models.GlobalWalletTransferState.GetName(() => IntegrationEvents.Models.GlobalWalletTransferState.FxFailed)
            }
        } });

        // Act
        var transactions = await _globalEquityQueries.GetWithdrawTransactions(new PaginateRequest(1, 10, null, null), null);

        // Assert
        Assert.IsType<WithdrawTransaction>(transactions.First());
        Assert.Equal(withdrawState.CorrelationId, transactions.First().Id);
    }

    [Theory]
    [InlineData(ProductType.ThaiEquity, 1)]
    [InlineData(ProductType.GlobalEquity, 1)]
    public async Task GlobalEquityQueries_CountDepositTransactions_ShouldReturnExpected_When_FilterByProduct(ProductType? productType, int expected)
    {
        // Arrange
        var filters = new TransactionFilters(null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, productType);
        _depositRepositoryMock.Setup(q => q.CountGlobalTransactions(It.IsAny<IQueryFilter<GlobalDepositTransaction>>()))
            .ReturnsAsync(1);

        // Act
        var actual = await _globalEquityQueries.CountDepositTransactions(filters);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(ProductType.ThaiEquity, 1)]
    [InlineData(ProductType.GlobalEquity, 1)]
    public async Task GlobalEquityQueries_CountWithdrawTransactions_ShouldReturnExpected_When_FilterByProduct(ProductType? productType, int expected)
    {
        // Arrange
        var filters = new TransactionFilters(null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, productType);
        _withdrawRepositoryMock.Setup(q => q.CountGlobalTransactions(It.IsAny<IQueryFilter<GlobalWithdrawTransaction>>()))
            .ReturnsAsync(1);

        // Act
        var actual = await _globalEquityQueries.CountWithdrawTransactions(filters);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GlobalEquityQueries_GetDepositTransaction_ShouldReturnExpectedDepositTransaction()
    {
        // Arrange
        var transactionNo = "GEDP2023092100001";
        var depositState = new DepositState()
        {
            PaymentReceivedAmount = 100,
            CorrelationId = Guid.NewGuid(),
            CurrentState = "DepositCompleted",
        };
        var globalTransfer = new GlobalWalletTransferState()
        {
            RequestedAmount = 1,
            CurrentState =
                IntegrationEvents.Models.GlobalWalletTransferState.GetName(() =>
                    IntegrationEvents.Models.GlobalWalletTransferState.FxFailed)
        };
        _depositRepositoryMock.Setup(q => q.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(depositState);
        _globalWalletDepositRepositoryMock.Setup(q => q.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(globalTransfer);

        // Act
        var actual = await _globalEquityQueries.GetDepositTransaction(transactionNo);

        // Assert
        Assert.IsType<DepositTransaction>(actual);
        Assert.Equal(depositState.PaymentReceivedAmount, actual.Amount);
        Assert.Equal(globalTransfer.CurrentState, actual.CurrentState);
    }

    [Fact]
    public async Task GlobalEquityQueries_GetDepositTransaction_ShouldReturnNull_When_DepositStateNotFound()
    {
        // Arrange
        var transactionNo = "GEDP2023092100001";
        _depositRepositoryMock.Setup(q => q.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync((DepositState?)null);

        // Act
        var actual = await _globalEquityQueries.GetDepositTransaction(transactionNo);

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async Task GlobalEquityQueries_GetDepositTransaction_ShouldReturnNull_When_GlobalTransferStateNotFound()
    {
        // Arrange
        var transactionNo = "GEDP2023092100001";
        var depositState = new DepositState();
        _depositRepositoryMock.Setup(q => q.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(depositState);
        _globalWalletDepositRepositoryMock.Setup(q => q.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync((GlobalWalletTransferState?)null);

        // Act
        var actual = await _globalEquityQueries.GetDepositTransaction(transactionNo);

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async Task GlobalEquityQueries_GetWithdrawTransaction_ShouldReturnExpectedWithdrawTransaction()
    {
        // Arrange
        var transactionNo = "GEWS2023092100001";
        var withdrawState = new WithdrawState()
        {
            CorrelationId = Guid.NewGuid(),
            CurrentState = "Final",
        };
        var globalTransfer = new GlobalWalletTransferState()
        {
            CorrelationId = Guid.NewGuid(),
            RequestedAmount = 1,
            CurrentState =
                IntegrationEvents.Models.GlobalWalletTransferState.GetName(() =>
                    IntegrationEvents.Models.GlobalWalletTransferState.FxFailed)
        };
        _withdrawRepositoryMock.Setup(q => q.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(withdrawState);
        _globalWalletDepositRepositoryMock.Setup(q => q.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(globalTransfer);

        // Act
        var actual = await _globalEquityQueries.GetWithdrawTransaction(transactionNo);

        // Assert
        Assert.IsType<WithdrawTransaction>(actual);
        Assert.Equal(withdrawState.CorrelationId, actual.Id);
        Assert.Equal(globalTransfer.CurrentState, actual.CurrentState);
    }

    [Fact]
    public async Task GlobalEquityQueries_GetWithdrawTransaction_ShouldReturnNull_When_WithdrawStateNotFound()
    {
        // Arrange
        var transactionNo = "GEWS2023092100001";
        _withdrawRepositoryMock.Setup(q => q.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync((WithdrawState?)null);

        // Act
        var actual = await _globalEquityQueries.GetWithdrawTransaction(transactionNo);

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async Task GlobalEquityQueries_GetWithdrawTransaction_ShouldReturnNull_When_GlobalTransferStateNotFound()
    {
        // Arrange
        var transactionNo = "GEWS2023092100001";
        var withdrawState = new WithdrawState();
        _withdrawRepositoryMock.Setup(q => q.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(withdrawState);
        _globalWalletDepositRepositoryMock.Setup(q => q.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync((GlobalWalletTransferState?)null);

        // Act
        var actual = await _globalEquityQueries.GetWithdrawTransaction(transactionNo);

        // Assert
        Assert.Null(actual);
    }
}
