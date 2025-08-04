using System.Linq.Expressions;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Application.Queries.Filters;
using Pi.WalletService.Domain.AggregatesModel.WithdrawAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Tests.Queries;

public class ThaiWithdrawFiltersTest
{
    [Fact]
    public void Should_Return_ThaiWithdrawTransactionExpressions_When_GetExpressions()
    {
        // Arrange
        var filters = new ThaiWithdrawFilters();

        // Act
        var actual = filters.GetExpressions();

        // Assert
        Assert.IsType<List<Expression<Func<ThaiWithdrawTransaction, bool>>>>(actual);
    }

    [Fact]
    public void Should_Have_A_Expression_When_GetExpressions_WithoutFilter()
    {
        // Arrange
        var expected = 3;
        var filters = new ThaiWithdrawFilters();

        // Act
        var actual = filters.GetExpressions();

        // Assert
        Assert.Equal(expected, actual.Count);
    }

    [Fact]
    public void Should_Return_ExpressionAmountAsExpected_When_GetExpressions()
    {
        // Arrange
        var expected = 15;
        var filters = new ThaiWithdrawFilters()
        {
            Channel = Channel.QR,
            Product = Product.Cash,
            State = "state",
            Status = TransactionStatus.Fail,
            BankCode = "BankCode",
            BankAccountNo = "BankAccountNo",
            CustomerCode = "CustomerCode",
            AccountCode = "AccountCode",
            TransactionNo = "TransactionNo",
            EffectiveDateFrom = new DateTime(),
            EffectiveDateTo = new DateTime(),
            CreatedAtFrom = new DateTime(),
            CreatedAtTo = new DateTime(),
            UserId = "userId"
        };

        // Act
        var actual = filters.GetExpressions();

        // Assert
        Assert.Equal(expected, actual.Count);
    }
}
