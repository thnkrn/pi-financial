using System.Linq.Expressions;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Application.Queries.Filters;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using CashDepositState = Pi.WalletService.Domain.AggregatesModel.CashAggregate.CashDepositState;

namespace Pi.WalletService.Application.Tests.Queries;

public class CashDepositFiltersTest
{
    [Fact]
    public void Should_Return_CashDepositStateExpressions_When_GetExpressions()
    {
        // Arrange
        var filters = new CashDepositFilters();

        // Act
        var actual = filters.GetExpressions();

        // Assert
        Assert.IsType<List<Expression<Func<CashDepositState, bool>>>>(actual);
    }

    [Fact]
    public void Should_Have_A_Expression_When_GetExpressions_WithoutFilter()
    {
        // Arrange
        var expected = 1;
        var filters = new CashDepositFilters();

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
        var filters = new CashDepositFilters()
        {
            Channel = Channel.QR,
            Product = Product.Cash,
            State = "state",
            Status = TransactionStatus.Fail,
            BankCode = "BankCode",
            BankName = "KKP",
            BankAccountNo = "BankAccountNo",
            CustomerCode = "CustomerCode",
            AccountCode = "AccountCode",
            TransactionNo = "TransactionNo",
            EffectiveDateFrom = new DateTime(),
            EffectiveDateTo = new DateTime(),
            PaymentReceivedFrom = new DateTime(),
            PaymentReceivedTo = new DateTime(),
            CreatedAtFrom = new DateTime(),
            CreatedAtTo = new DateTime(),
        };

        // Act
        var actual = filters.GetExpressions();

        // Assert
        Assert.Equal(expected, actual.Count);
    }
}
