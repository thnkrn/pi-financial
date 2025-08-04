using System.Linq.Expressions;
using Pi.WalletService.Domain.AggregatesModel.DepositAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using DepositState = Pi.WalletService.Domain.AggregatesModel.DepositAggregate.DepositState;

namespace Pi.WalletService.Domain.Tests.AggregatesModel.DepositAggregate;

public class DepositStateFiltersTest
{
    [Fact]
    public void Should_Return_GlobalDepositTransactionExpressions_When_GetExpressions()
    {
        // Arrange
        var filters = new DepositStateFilters();

        // Act
        var actual = filters.GetExpressions();

        // Assert
        Assert.IsType<List<Expression<Func<DepositState, bool>>>>(actual);
    }

    [Fact]
    public void Should_Have_A_Expression_When_GetExpressions_WithoutFilter()
    {
        // Arrange
        var expected = 1;
        var filters = new DepositStateFilters();

        // Act
        var actual = filters.GetExpressions();

        // Assert
        Assert.Equal(expected, actual.Count);
    }

    [Fact]
    public void Should_Return_ExpressionAmountAsExpected_When_GetExpressions()
    {
        // Arrange
        var expected = 16;
        var filters = new DepositStateFilters()
        {
            Product = Product.Cash,
            Products = new[] { Product.Derivatives, Product.Cash },
            Channel = Channel.QR,
            State = "state",
            States = new[] { "someState" },
            BankCode = "BankCode",
            BankAccountNo = "BankAccountNo",
            CustomerCode = "CustomerCode",
            AccountCode = "AccountCode",
            TransactionNo = "TransactionNo",
            PaymentReceivedFrom = new DateTime(),
            PaymentReceivedTo = new DateTime(),
            CreatedAtFrom = new DateTime(),
            CreatedAtTo = new DateTime(),
            UserId = "someUserId"
        };

        // Act
        var actual = filters.GetExpressions();

        // Assert
        Assert.Equal(expected, actual.Count);
    }
}
