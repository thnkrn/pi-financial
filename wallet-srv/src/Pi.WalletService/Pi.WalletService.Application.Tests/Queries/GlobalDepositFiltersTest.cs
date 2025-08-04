using System.Linq.Expressions;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Application.Queries.Filters;
using Pi.WalletService.Domain.AggregatesModel.DepositAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Tests.Queries;

public class GlobalDepositFiltersTest
{
    [Fact]
    public void Should_Return_GlobalDepositTransactionExpressions_When_GetExpressions()
    {
        // Arrange
        var filters = new GlobalDepositFilters();

        // Act
        var actual = filters.GetExpressions();

        // Assert
        Assert.IsType<List<Expression<Func<GlobalDepositTransaction, bool>>>>(actual);
    }

    [Fact]
    public void Should_Have_A_Expression_When_GetExpressions_WithoutFilter()
    {
        // Arrange
        var expected = 3;
        var filters = new GlobalDepositFilters();

        // Act
        var actual = filters.GetExpressions();

        // Assert
        Assert.Equal(expected, actual.Count);
    }

    [Fact]
    public void Should_Return_ExpressionAmountAsExpected_When_GetExpressions()
    {
        // Arrange
        var expected = 18;
        var filters = new GlobalDepositFilters()
        {
            Channel = Channel.QR,
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
