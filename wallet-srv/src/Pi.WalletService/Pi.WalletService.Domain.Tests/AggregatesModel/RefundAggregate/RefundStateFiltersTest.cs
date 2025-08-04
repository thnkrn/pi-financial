using Pi.WalletService.Domain.AggregatesModel.RefundAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Domain.Tests.AggregatesModel.RefundAggregate;

public class RefundStateFiltersTest
{
    [Fact]
    public void Should_Return_Expressions_When_GetExpressions_And_FiltersNotNull()
    {
        // Arrange
        var filters = new RefundStateFilters()
        {
            Channel = Channel.QR,
            Product = Product.Cash,
            State = "someState",
            States = new[] { "someState2" },
            UserId = "userId",
            BankCode = "bankCode",
            BankAccountNo = "bankAccountNo",
            CustomerCode = "customerCode",
            AccountCode = "accountCode",
            TransactionNo = "transactionNo",
            RefundAtFrom = DateTime.Today,
            RefundAtTo = DateTime.Today,
            CreatedAtFrom = DateTime.Today,
            CreatedAtTo = DateTime.Today,
        };

        // Act
        var expressions = filters.GetExpressions();

        // Assert
        Assert.NotEmpty(expressions);
    }

    [Fact]
    public void Should_Return_Empty_When_GetExpressions_And_FiltersNull()
    {
        // Arrange
        var expected = 0;
        var filters = new RefundStateFilters();

        // Act
        var expressions = filters.GetExpressions();

        // Assert
        Assert.Equal(expected, expressions.Count);
    }
}
