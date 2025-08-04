using Moq;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Application.Queries.Filters;
using Pi.WalletService.Domain;
using Pi.WalletService.Domain.AggregatesModel.CashAggregate;
using Pi.WalletService.Domain.AggregatesModel.DepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Tests.Queries;

public class ThaiEquityQueriesTest
{
    private readonly Mock<ICashDepositRepository> _cashDepositRepositoryMock;
    private readonly Mock<IDepositRepository> _depositRepositoryMock;
    private readonly Mock<IWithdrawRepository> _withdrawRepositoryMock;
    private readonly Mock<ICashWithdrawRepository> _cashWithdrawRepositoryMock;
    private readonly ThaiEquityQueries _thaiEquityQueries;

    public ThaiEquityQueriesTest()
    {
        _depositRepositoryMock = new Mock<IDepositRepository>();
        _withdrawRepositoryMock = new Mock<IWithdrawRepository>();
        _cashDepositRepositoryMock = new Mock<ICashDepositRepository>();
        _cashWithdrawRepositoryMock = new Mock<ICashWithdrawRepository>();
        _thaiEquityQueries = new ThaiEquityQueries(
            _depositRepositoryMock.Object,
            _withdrawRepositoryMock.Object,
            _cashDepositRepositoryMock.Object,
            _cashWithdrawRepositoryMock.Object);
    }

    [Theory]
    [InlineData(Channel.EForm)]
    [InlineData(Channel.QR)]
    [InlineData(Channel.OnlineViaKKP)]
    [InlineData(Channel.TransferApp)]
    [InlineData(Channel.ATS)]
    [InlineData(null)]
    public async Task ThaiEquityQueries_GetDepositTransactions_With_ChannelFilter_ShouldReturnExpectedDepositTransactions(Channel? channel)
    {
        // Arrange
        var deposit = new DepositState() { CorrelationId = Guid.NewGuid() };
        var cashDeposit = new CashDepositState() { CorrelationId = Guid.NewGuid() };
        var filters = new TransactionFilters(channel, null, null, null, null, null, null, null, null, null, null, null,
            null);
        _depositRepositoryMock.Setup(q => q.GetThaiDeposit(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<ThaiDepositFilters>()
        )).ReturnsAsync(new List<ThaiDepositTransaction> { new() {
            DepositState = deposit,
            CashDepositState = cashDeposit
        }});

        // Act
        var transactions = await _thaiEquityQueries.GetDepositTransactions(new PaginateRequest(1, 10, null, null), filters);

        // Assert
        Assert.IsType<DepositTransaction>(transactions.First());
        Assert.Equal(deposit.CorrelationId, transactions.First().Id);
    }

    [Fact]
    public async Task ThaiEquityQueries_GetDepositTransactions_With_ChannelFilterEqualSetTrade_ShouldReturnExpectedDepositTransactions()
    {
        // Arrange
        var cashDeposit = new CashDepositState() { CorrelationId = Guid.NewGuid() };
        var filters = new TransactionFilters(Channel.SetTrade, null, null, null, null, null, null, null, null, null, null, null,
            null);
        _cashDepositRepositoryMock.Setup(q => q.Get(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CashDepositFilters>()
        )).ReturnsAsync(new List<CashDepositState> { cashDeposit });

        // Act
        var transactions = await _thaiEquityQueries.GetDepositTransactions(new PaginateRequest(1, 10, null, null), filters);

        // Assert
        Assert.IsType<DepositTransaction>(transactions.First());
        Assert.Equal(cashDeposit.CorrelationId, transactions.First().Id);
    }

    [Theory]
    [InlineData(Channel.EForm)]
    [InlineData(Channel.QR)]
    [InlineData(Channel.OnlineViaKKP)]
    [InlineData(Channel.TransferApp)]
    [InlineData(Channel.ATS)]
    [InlineData(null)]
    public async Task ThaiEquityQueries_GetDepositTransactions_With_ChannelFilter_ShouldNotCallCashDepositRepo(Channel? channel)
    {
        // Arrange
        var filters = new TransactionFilters(channel, null, null, null, null, null, null, null, null, null, null, null,
            null);
        _depositRepositoryMock.Setup(q => q.GetThaiDeposit(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<ThaiDepositFilters>()
        )).ReturnsAsync(new List<ThaiDepositTransaction> { new() {
            DepositState = new DepositState(),
            CashDepositState = new CashDepositState()
        }}).Verifiable();
        _cashDepositRepositoryMock.Setup(q => q.Get(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CashDepositFilters>()
        )).Verifiable();

        // Act
        await _thaiEquityQueries.GetDepositTransactions(new PaginateRequest(1, 10, null, null), filters);

        // Assert
        _depositRepositoryMock.Verify(q => q.GetThaiDeposit(It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<ThaiDepositFilters>()
        ), Times.Once);
        _cashDepositRepositoryMock.Verify(q => q.Get(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CashDepositFilters>()
        ), Times.Never);
    }

    [Fact]
    public async Task ThaiEquityQueries_GetDepositTransactions_With_ChannelFilterEqualSetTrade_ShouldNotCallDepositRepo()
    {
        // Arrange
        var filters = new TransactionFilters(Channel.SetTrade, null, null, null, null, null, null, null, null, null, null, null,
            null);
        _depositRepositoryMock.Setup(q => q.Get(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<DepositStateFilters>()
        )).Verifiable();
        _cashDepositRepositoryMock.Setup(q => q.Get(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CashDepositFilters>()
        )).ReturnsAsync(new List<CashDepositState> { new() }).Verifiable();

        // Act
        await _thaiEquityQueries.GetDepositTransactions(new PaginateRequest(1, 10, null, null), filters);

        // Assert
        _depositRepositoryMock.Verify(q => q.Get(It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<DepositStateFilters>()
            ), Times.Never);
        _cashDepositRepositoryMock.Verify(q => q.Get(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CashDepositFilters>()
        ), Times.Once);
    }

    [Fact]
    public async Task ThaiEquityQueries_GetDepositTransactions_Without_Filter_ShouldReturnExpectedDepositTransactions()
    {
        // Arrange
        var depositTransaction = new ThaiDepositTransaction
        {
            DepositState = new DepositState() { CorrelationId = Guid.NewGuid() },
            CashDepositState = new CashDepositState() { CorrelationId = Guid.NewGuid() }
        };
        _depositRepositoryMock.Setup(q => q.GetThaiDeposit(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<ThaiDepositFilters>()
        )).ReturnsAsync(new List<ThaiDepositTransaction> { depositTransaction });

        // Act
        var transactions = await _thaiEquityQueries.GetDepositTransactions(new PaginateRequest(1, 10, null, null), null);

        // Assert
        Assert.IsType<DepositTransaction>(transactions.First());
        Assert.Equal(depositTransaction.DepositState.CorrelationId, transactions.First().Id);
    }

    [Fact]
    public async Task ThaiEquityQueries_GetWithdrawTransactions_ShouldReturnExpectedWithdrawTransactions()
    {
        // Arrange
        var filters = new TransactionFilters(null, Product.Cash, null, null, null, null, null, null, null, null, null, null,
            null);
        var thaiTransaction = new ThaiWithdrawTransaction()
        {
            WithdrawState = new WithdrawState() { CorrelationId = Guid.NewGuid() },
            CashWithdrawState = new CashWithdrawState() { CorrelationId = Guid.NewGuid() }
        };
        _withdrawRepositoryMock.Setup(q => q.GetThaiWithdraw(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<ThaiWithdrawFilters>()
        )).ReturnsAsync(new List<ThaiWithdrawTransaction> { thaiTransaction });

        // Act
        var transactions = await _thaiEquityQueries.GetWithdrawTransactions(new PaginateRequest(1, 10, null, null), filters);

        // Assert
        Assert.IsType<WithdrawTransaction>(transactions.First());
        Assert.Equal(thaiTransaction.WithdrawState.CorrelationId, transactions.First().Id);
    }

    [Fact]
    public async Task ThaiEquityQueries_GetWithdrawTransactions_Without_Filter_ShouldReturnExpectedWithdrawTransactions()
    {
        // Arrange
        var thaiTransaction = new ThaiWithdrawTransaction()
        {
            WithdrawState = new WithdrawState() { CorrelationId = Guid.NewGuid() },
            CashWithdrawState = new CashWithdrawState() { CorrelationId = Guid.NewGuid() }
        }; _withdrawRepositoryMock.Setup(q => q.GetThaiWithdraw(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<ThaiWithdrawFilters>()
        )).ReturnsAsync(new List<ThaiWithdrawTransaction> { thaiTransaction });

        // Act
        var transactions = await _thaiEquityQueries.GetWithdrawTransactions(new PaginateRequest(1, 10, null, null), null);

        // Assert
        Assert.IsType<WithdrawTransaction>(transactions.First());
        Assert.Equal(thaiTransaction.WithdrawState.CorrelationId, transactions.First().Id);
    }

    [Theory]
    [InlineData(Channel.EForm)]
    [InlineData(Channel.QR)]
    [InlineData(Channel.OnlineViaKKP)]
    [InlineData(Channel.TransferApp)]
    [InlineData(Channel.ATS)]
    [InlineData(null)]
    public async Task ThaiEquityQueries_CountDepositTransactions_With_ChannelFilter_ShouldReturnExpectedAmount(Channel? channel)
    {
        // Arrange
        var depositAmount = 1;
        var cashDepositAmount = 2;
        var filters = new TransactionFilters(channel, null, null, null, null, null, null, null, null, null, null, null,
            null);
        _depositRepositoryMock.Setup(q => q.CountThaiTransactions(It.IsAny<ThaiDepositFilters>()))
            .ReturnsAsync(depositAmount).Verifiable();
        _cashDepositRepositoryMock.Setup(q => q.CountTransactions(It.IsAny<CashDepositFilters>()))
            .ReturnsAsync(cashDepositAmount).Verifiable();

        // Act
        var actual = await _thaiEquityQueries.CountDepositTransactions(filters);

        // Assert
        Assert.Equal(depositAmount, actual);
    }

    [Fact]
    public async Task ThaiEquityQueries_CountDepositTransactions_With_ChannelFilterEqualSetTrade_ShouldReturnExpectedAmount()
    {
        // Arrange
        var depositAmount = 1;
        var cashDepositAmount = 2;
        var filters = new TransactionFilters(Channel.SetTrade, null, null, null, null, null, null, null, null, null, null, null,
            null);
        _depositRepositoryMock.Setup(q => q.CountTransactions(It.IsAny<IQueryFilter<DepositState>>()))
            .ReturnsAsync(depositAmount).Verifiable();
        _cashDepositRepositoryMock.Setup(q => q.CountTransactions(It.IsAny<IQueryFilter<CashDepositState>>()))
            .ReturnsAsync(cashDepositAmount).Verifiable();

        // Act
        var actual = await _thaiEquityQueries.CountDepositTransactions(filters);

        // Assert
        Assert.Equal(cashDepositAmount, actual);
    }

    [Theory]
    [InlineData(Channel.EForm)]
    [InlineData(Channel.QR)]
    [InlineData(Channel.OnlineViaKKP)]
    [InlineData(Channel.TransferApp)]
    [InlineData(Channel.ATS)]
    [InlineData(null)]
    public async Task ThaiEquityQueries_CountDepositTransactions_With_ChannelFilter_ShouldNotCallCashDepositRepo(Channel? channel)
    {
        // Arrange
        var filters = new TransactionFilters(channel, null, null, null, null, null, null, null, null, null, null, null,
            null);
        _depositRepositoryMock.Setup(q => q.CountThaiTransactions(It.IsAny<ThaiDepositFilters>()))
            .ReturnsAsync(1).Verifiable();
        _cashDepositRepositoryMock.Setup(q => q.CountTransactions(It.IsAny<CashDepositFilters>()))
            .ReturnsAsync(1).Verifiable();

        // Act
        await _thaiEquityQueries.CountDepositTransactions(filters);

        // Assert
        _depositRepositoryMock.Verify(q => q.CountThaiTransactions(It.IsAny<ThaiDepositFilters>()), Times.Once);
        _cashDepositRepositoryMock.Verify(q => q.CountTransactions(It.IsAny<CashDepositFilters>()), Times.Never);
    }

    [Fact]
    public async Task ThaiEquityQueries_CountDepositTransactions_With_ChannelFilter_ShouldNotCallDepositRepo()
    {
        // Arrange
        var filters = new TransactionFilters(Channel.SetTrade, null, null, null, null, null, null, null, null, null, null, null,
            null);
        _depositRepositoryMock.Setup(q => q.CountTransactions(It.IsAny<IQueryFilter<DepositState>>()))
            .ReturnsAsync(1).Verifiable();
        _cashDepositRepositoryMock.Setup(q => q.CountTransactions(It.IsAny<IQueryFilter<CashDepositState>>()))
            .ReturnsAsync(1).Verifiable();

        // Act
        await _thaiEquityQueries.CountDepositTransactions(filters);

        // Assert
        _depositRepositoryMock.Verify(q => q.CountTransactions(It.IsAny<IQueryFilter<DepositState>>()), Times.Never);
        _cashDepositRepositoryMock.Verify(q => q.CountTransactions(It.IsAny<IQueryFilter<CashDepositState>>()), Times.Once);
    }

    [Theory]
    [InlineData(Product.Cash, 1)]
    [InlineData(Product.Crypto, 1)]
    public async Task ThaiEquityQueries_CountDepositTransactions_ShouldReturnExpected_When_FilterByProduct(Product? product, int expected)
    {
        // Arrange
        var filters = new TransactionFilters(null, product, null, null, null, null, null, null, null, null, null, null,
            null);
        _depositRepositoryMock.Setup(q => q.CountThaiTransactions(It.IsAny<ThaiDepositFilters>()))
            .ReturnsAsync(1);

        // Act
        var actual = await _thaiEquityQueries.CountDepositTransactions(filters);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(Product.Cash, 1)]
    [InlineData(Product.Crypto, 1)]
    public async Task ThaiEquityQueries_CountWithdrawTransactions_ShouldReturnExpected_When_FilterByProduct(Product? product, int expected)
    {
        // Arrange
        var filters = new TransactionFilters(null, product, null, null, null, null, null, null, null, null, null, null,
            null);
        _withdrawRepositoryMock.Setup(q => q.CountThaiTransactions(It.IsAny<ThaiWithdrawFilters>()))
            .ReturnsAsync(1);

        // Act
        var actual = await _thaiEquityQueries.CountWithdrawTransactions(filters);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("DHDP2023092100001")]
    [InlineData("00123SetTrade")]
    public async Task ThaiEquityQueries_GetDepositTransaction_ShouldReturnDepositTransaction(string transactionNo)
    {
        // Arrange
        var depositState = new DepositState();
        var cashDeposit = new CashDepositState();
        _depositRepositoryMock.Setup(q => q.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(depositState);
        _cashDepositRepositoryMock.Setup(q => q.Get(It.IsAny<string>()))
            .ReturnsAsync(cashDeposit);

        // Act
        var actual = await _thaiEquityQueries.GetDepositTransaction(transactionNo);

        // Assert
        Assert.IsType<DepositTransaction>(actual);
    }

    [Fact]
    public async Task ThaiEquityQueries_GetDepositTransaction_ShouldReturnExpectedDepositTransaction_When_TransactionNoIsNotSetTrade()
    {
        // Arrange
        var transactionNo = "DHDP2023092100001";
        var depositState = new DepositState() { RequestedAmount = 111, PaymentReceivedAmount = 999 };
        var cashDeposit = new CashDepositState() { RequestedAmount = 100 };
        _depositRepositoryMock.Setup(q => q.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(depositState);
        _cashDepositRepositoryMock.Setup(q => q.Get(It.IsAny<string>()))
            .ReturnsAsync(cashDeposit);

        // Act
        var actual = await _thaiEquityQueries.GetDepositTransaction(transactionNo);

        // Assert
        Assert.Equal(depositState.PaymentReceivedAmount, actual!.Amount);
    }

    [Fact]
    public async Task ThaiEquityQueries_GetDepositTransaction_ShouldReturnExpectedDepositTransaction_When_TransactionNoIsSetTrade()
    {
        // Arrange
        var transactionNo = "00123SetTrade";
        var depositState = new DepositState() { RequestedAmount = 111, PaymentReceivedAmount = 999 };
        var cashDeposit = new CashDepositState() { RequestedAmount = 100 };
        _depositRepositoryMock.Setup(q => q.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(depositState);
        _cashDepositRepositoryMock.Setup(q => q.Get(It.IsAny<string>()))
            .ReturnsAsync(cashDeposit);

        // Act
        var actual = await _thaiEquityQueries.GetDepositTransaction(transactionNo);

        // Assert
        Assert.Equal(cashDeposit.RequestedAmount, actual!.Amount);
    }

    [Theory]
    [InlineData("DHDP2023092100001")]
    [InlineData("00123SetTrade")]
    public async Task ThaiEquityQueries_GetDepositTransaction_ShouldReturnNull_When_DepositStateNotFound(string transactionNo)
    {
        // Arrange
        _depositRepositoryMock.Setup(q => q.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync((DepositState?)null);
        _cashDepositRepositoryMock.Setup(q => q.Get(It.IsAny<string>()))
            .ReturnsAsync((CashDepositState?)null);

        // Act
        var actual = await _thaiEquityQueries.GetDepositTransaction(transactionNo);

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async Task ThaiEquityQueries_GetDepositTransaction_ShouldReturnDepositTransaction_When_CashDepositStateNotFound()
    {
        // Arrange
        var transactionNo = "DHDP2023092100001";
        var depositState = new DepositState() { RequestedAmount = 111, PaymentReceivedAmount = 999 };
        _depositRepositoryMock.Setup(q => q.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(depositState);
        _cashDepositRepositoryMock.Setup(q => q.Get(It.IsAny<string>()))
            .ReturnsAsync((CashDepositState?)null);

        // Act
        var actual = await _thaiEquityQueries.GetDepositTransaction(transactionNo);

        // Assert
        Assert.IsType<DepositTransaction>(actual);
        Assert.Equal(depositState.PaymentReceivedAmount, actual!.Amount);
    }

    [Fact]
    public async Task ThaiEquityQueries_GetDepositTransaction_ShouldCallCashDepositOnly_When_TransactionNoIsSetTrade()
    {
        // Arrange
        var transactionNo = "00123SetTrade";
        _depositRepositoryMock.Setup(q => q.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(new DepositState()).Verifiable();
        _cashDepositRepositoryMock.Setup(q => q.Get(It.IsAny<string>()))
            .ReturnsAsync(new CashDepositState()).Verifiable();

        // Act
        await _thaiEquityQueries.GetDepositTransaction(transactionNo);

        // Assert
        _cashDepositRepositoryMock.Verify(q => q.Get(It.IsAny<string>()), Times.Once);
        _depositRepositoryMock.Verify(q => q.GetByTransactionNo(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ThaiEquityQueries_GetDepositTransaction_ShouldCallDepositAndCashDeposit_When_TransactionNoIsNotSetTrade()
    {
        // Arrange
        var transactionNo = "DHDP2023092100001";
        _depositRepositoryMock.Setup(q => q.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(new DepositState()).Verifiable();
        _cashDepositRepositoryMock.Setup(q => q.Get(It.IsAny<string>()))
            .ReturnsAsync(new CashDepositState()).Verifiable();

        // Act
        await _thaiEquityQueries.GetDepositTransaction(transactionNo);

        // Assert
        _cashDepositRepositoryMock.Verify(q => q.Get(It.IsAny<string>()), Times.Once);
        _depositRepositoryMock.Verify(q => q.GetByTransactionNo(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ThaiEquityQueries_GetWithdrawTransaction_ShouldReturnExpectedWithdrawTransaction()
    {
        // Arrange
        var transactionNo = "00123SetTrade";
        var withdrawState = new WithdrawState() { TransactionNo = "someNumber" };
        var cashWithdraw = new CashWithdrawState() { TransactionNo = "shouldBeSame" };
        _withdrawRepositoryMock.Setup(q => q.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(withdrawState);
        _cashWithdrawRepositoryMock.Setup(q => q.Get(It.IsAny<string>()))
            .ReturnsAsync(cashWithdraw);

        // Act
        var actual = await _thaiEquityQueries.GetWithdrawTransaction(transactionNo);

        // Assert
        Assert.Equal(withdrawState.TransactionNo, actual!.TransactionNo);
    }

    [Fact]
    public async Task ThaiEquityQueries_GetWithdrawTransaction_ShouldReturnNull_When_WithdrawStateNotFound()
    {
        // Arrange
        var transactionNo = "00123SetTrade";
        var cashWithdraw = new CashWithdrawState() { TransactionNo = "shouldBeSame" };
        _withdrawRepositoryMock.Setup(q => q.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync((WithdrawState?)null);
        _cashWithdrawRepositoryMock.Setup(q => q.Get(It.IsAny<string>()))
            .ReturnsAsync(cashWithdraw);

        // Act
        var actual = await _thaiEquityQueries.GetWithdrawTransaction(transactionNo);

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async Task ThaiEquityQueries_GetWithdrawTransaction_ShouldReturnNull_When_CashWithdrawStateNotFound()
    {
        // Arrange
        var transactionNo = "00123SetTrade";
        var withdrawState = new WithdrawState() { TransactionNo = "someNumber" };
        _withdrawRepositoryMock.Setup(q => q.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(withdrawState);
        _cashWithdrawRepositoryMock.Setup(q => q.Get(It.IsAny<string>()))
            .ReturnsAsync((CashWithdrawState?)null);

        // Act
        var actual = await _thaiEquityQueries.GetWithdrawTransaction(transactionNo);

        // Assert
        Assert.Null(actual);
    }
}
