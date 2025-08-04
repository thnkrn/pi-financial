using Pi.SetService.Domain.AggregatesModel.AccountAggregate;

namespace Pi.SetService.Domain.Tests.AggregatesModel.AccountAggregate;

public class AvailableCreditBalanceTest
{
    [Fact]
    public void Should_ReturnExpected_When_Init()
    {
        // Act
        var actual = new AvailableCreditBalance
        {
            Liability = 828.00m,
            Asset = 501448.22m,
            Equity = 500620.22m,
            MarginRequired = 26677,
            ExcessEquity = 473943.22m,
            TradingAccountNo = "0801078-6",
            AccountNo = "08010786",
            TraderId = "3621",
            CreditLimit = 500000,
            BuyCredit = 947886.44m,
            CashBalance = 448922.22m,
            CallForce = 40523.75m,
            CallMargin = 47603.85m
        };

        // Assert
        Assert.Equal(99.8349m, decimal.Round(actual.MmPercentage, 4));
    }
}