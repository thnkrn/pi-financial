using System.Linq.Expressions;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Application.Queries.Filters;
using Pi.WalletService.Domain.AggregatesModel.WithdrawAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Tests.Queries;

public class GlobalWithdrawFiltersTest
{
    [Fact]
    public void Should_Return_GlobalWithdrawTransactionExpressions_When_GetExpressions()
    {
        // Arrange
        var filters = new GlobalWithdrawFilters();

        // Act
        var actual = filters.GetExpressions();

        // Assert
        Assert.IsType<List<Expression<Func<GlobalWithdrawTransaction, bool>>>>(actual);
    }

    [Fact]
    public void Should_Have_A_Expression_When_GetExpressions_WithoutFilter()
    {
        // Arrange
        var expected = 3;
        var filters = new GlobalWithdrawFilters();

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
        var filters = new GlobalWithdrawFilters()
        {
            Channel = Channel.QR,
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
        };

        // Act
        var actual = filters.GetExpressions();

        // Assert
        Assert.Equal(expected, actual.Count);
    }
}
