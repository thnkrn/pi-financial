using System.Linq.Expressions;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Application.Queries.Filters;
using Pi.WalletService.Domain.AggregatesModel.DepositAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Tests.Queries;

public class ThaiDepositFiltersTest
{
    [Fact]
    public void Should_Return_ThaiDepositTransactionExpressions_When_GetExpressions()
    {
        // Arrange
        var filters = new ThaiDepositFilters();

        // Act
        var actual = filters.GetExpressions();

        // Assert
        Assert.IsType<List<Expression<Func<ThaiDepositTransaction, bool>>>>(actual);
    }

    [Fact]
    public void Should_Have_A_Expression_When_GetExpressions_WithoutFilter()
    {
        // Arrange
        var expected = 2;
        var filters = new ThaiDepositFilters();

        // Act
        var actual = filters.GetExpressions();

        // Assert
        Assert.Equal(expected, actual.Count);
    }

    [Fact]
    public void Should_Return_ExpressionAmountAsExpected_When_GetExpressions()
    {
        // Arrange
        var expected = 19;
        var filters = new ThaiDepositFilters()
        {
            Channel = Channel.QR,
            Product = Product.Cash,
            State = "state",
            Status = TransactionStatus.Fail,
            BankCode = "BankCode",
            BankAccountNo = "BankAccountNo",
            BankName = "BankName",
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
