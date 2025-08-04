using Pi.WalletService.Application.Factories;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Tests.Factories;

public class EntityFactoryTest
{
    [Theory]
    [InlineData(ProductType.ThaiEquity)]
    [InlineData(ProductType.GlobalEquity)]
    public void Should_Return_Products_When_GetProducts(ProductType productType)
    {
        // Act
        var actual = EntityFactory.GetProducts(productType);

        // Assert
        Assert.NotEmpty(actual);
    }

    [Theory]
    [InlineData(TransactionStatus.Success, TransactionType.Withdraw)]
    [InlineData(TransactionStatus.Fail, TransactionType.Deposit)]
    [InlineData(TransactionStatus.Pending, null)]
    public void Should_Return_States_When_GetGlobalStates(TransactionStatus status, TransactionType? transactionType)
    {
        // Act
        var actual = EntityFactory.GetGlobalStates(status, transactionType);

        // Assert
        Assert.NotEmpty(actual);
    }

    [Theory]
    [InlineData(TransactionStatus.Success)]
    [InlineData(TransactionStatus.Fail)]
    [InlineData(TransactionStatus.Pending)]
    public void Should_Return_States_When_GetRefundStates(TransactionStatus status)
    {
        // Act
        var actual = EntityFactory.GetRefundStates(status);

        // Assert
        Assert.NotEmpty(actual);
    }

    [Theory]
    [InlineData(TransactionStatus.Success)]
    [InlineData(TransactionStatus.Fail)]
    [InlineData(TransactionStatus.Pending)]
    public void Should_Return_States_When_GetWithdrawStates(TransactionStatus status)
    {
        // Act
        var actual = EntityFactory.GetWithdrawStates(status);

        // Assert
        Assert.NotEmpty(actual);
    }

    [Theory]
    [InlineData(TransactionStatus.Success)]
    [InlineData(TransactionStatus.Fail)]
    [InlineData(TransactionStatus.Pending)]
    public void Should_Return_States_When_GetDepositStates(TransactionStatus status)
    {
        // Act
        var actual = EntityFactory.GetDepositStates(status);

        // Assert
        Assert.NotEmpty(actual);
    }

    [Theory]
    [InlineData(TransactionStatus.Success)]
    [InlineData(TransactionStatus.Pending)]
    public void Should_Return_States_When_GetCashDepositStates(TransactionStatus status)
    {
        // Act
        var actual = EntityFactory.GetCashDepositStates(status);

        // Assert
        Assert.NotEmpty(actual);
    }
}
