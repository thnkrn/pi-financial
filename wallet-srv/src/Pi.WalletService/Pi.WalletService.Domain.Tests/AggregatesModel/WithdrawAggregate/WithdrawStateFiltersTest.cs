using Pi.WalletService.Domain.AggregatesModel.WithdrawAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Domain.Tests.AggregatesModel.WithdrawAggregate;

public class WithdrawStateFiltersTest
{
    [Fact]
    public void Should_Return_Expressions_When_GetExpressions_And_FiltersNotNull()
    {
        // Arrange
        var filters = new WithdrawStateFilters()
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
            PaymentDisbursedDateTimeFrom = DateTime.Today,
            PaymentDisbursedDateTimeTo = DateTime.Today,
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
        var expected = 1;
        var filters = new WithdrawStateFilters();

        // Act
        var expressions = filters.GetExpressions();

        // Assert
        Assert.Equal(expected, expressions.Count);
    }
}
