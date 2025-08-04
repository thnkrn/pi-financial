using Pi.WalletService.Application.Utilities;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.Utilities;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Tests.Utilities;

public class RoundingUtilsTest
{
    [Theory]
    [InlineData(33.369, 33.37)]
    [InlineData(33.37, 33.37)]
    [InlineData(33.374, 33.37)]
    [InlineData(33.375, 33.38)]
    [InlineData(33.376, 33.38)]
    [InlineData(33.38, 33.38)]
    [InlineData(33.381, 33.38)]
    public void Should_Return_Correct_Result_When_RoundForLogic(decimal value, decimal expected)
    {
        var actual = RoundingUtils.RoundForLogic(value);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(TransactionType.Deposit, 33.37, 33.37)]
    [InlineData(TransactionType.Deposit, 33.375, 33.38)]
    [InlineData(TransactionType.Deposit, 33.38, 33.38)]
    [InlineData(TransactionType.Deposit, 33.381, 33.39)]
    [InlineData(TransactionType.Withdraw, 33.369, 33.36)]
    [InlineData(TransactionType.Withdraw, 33.37, 33.37)]
    [InlineData(TransactionType.Withdraw, 33.375, 33.37)]
    [InlineData(TransactionType.Withdraw, 33.38, 33.38)]
    public void Should_Return_Correct_Result_When_RoundExchangeRate(TransactionType transactionType, decimal value, decimal expected)
    {
        var actual = RoundingUtils.RoundExchangeRate(transactionType, value, 2);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(TransactionType.Deposit, 898.741761, Currency.USD, 898.74)]
    [InlineData(TransactionType.Deposit, 928.699820, Currency.USD, 928.69)]
    [InlineData(TransactionType.Deposit, 33380.3338, Currency.THB, 33380.33)]
    [InlineData(TransactionType.Deposit, 33413.0462, Currency.THB, 33413.05)]
    [InlineData(TransactionType.Withdraw, 898.741761, Currency.USD, 898.75)]
    [InlineData(TransactionType.Withdraw, 928.699820, Currency.USD, 928.70)]
    [InlineData(TransactionType.Withdraw, 33380.3338, Currency.THB, 33380.33)]
    [InlineData(TransactionType.Withdraw, 33413.0462, Currency.THB, 33413.04)]
    public void Should_Return_Correct_Result_When_RoundTransactionValue(TransactionType transactionType, decimal value, Currency currency, decimal expected)
    {
        var actual = RoundingUtils.RoundTransactionValue(transactionType, value, currency);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_Return_Same_Value_On_Same_Currency_When_RoundExchangeTransaction()
    {
        const decimal expected = 1234;
        var actual = RoundingUtils.RoundExchangeTransaction(TransactionType.Deposit, Currency.USD, expected, Currency.USD, 30);

        Assert.Equal(expected, actual);
    }

    // [Fact]
    // public void Should_Throw_When_No_THB_On_Any_Side_When_RoundExchangeTransaction()
    // {
    //     Assert.Throws<InvalidDataException>(() =>
    //     {
    //         RoundingUtils.RoundExchangeTransaction(
    //             TransactionType.Deposit,
    //             Currency.USD,
    //             123,
    //             Currency., // when there's more currency
    //             30
    //         );
    //     });
    // }

    [Theory]
    [InlineData(TransactionType.Deposit, Currency.USD, 1000.01, Currency.THB, 33.375, 33380.33)]
    [InlineData(TransactionType.Deposit, Currency.USD, 1000.99, Currency.THB, 33.375, 33413.05)]
    [InlineData(TransactionType.Deposit, Currency.THB, 30000, Currency.USD, 33.375, 898.74)]
    [InlineData(TransactionType.Deposit, Currency.THB, 31000, Currency.USD, 33.375, 928.69)]
    [InlineData(TransactionType.Withdraw, Currency.USD, 1000.01, Currency.THB, 33.385, 33380.33)]
    [InlineData(TransactionType.Withdraw, Currency.USD, 1000.99, Currency.THB, 33.385, 33413.04)]
    [InlineData(TransactionType.Withdraw, Currency.THB, 30000, Currency.USD, 33.385, 898.75)]
    [InlineData(TransactionType.Withdraw, Currency.THB, 31000, Currency.USD, 33.385, 928.70)]
    public void Should_Return_Correct_Result_When_RoundExchangeTransaction(
        TransactionType transactionType,
        Currency requestedCurrency,
        decimal inputAmount,
        Currency convertedCurrency,
        decimal exchangeRate,
        decimal expected
    )
    {
        var actual = RoundingUtils.RoundExchangeTransaction(transactionType, requestedCurrency, inputAmount, convertedCurrency, exchangeRate);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(Currency.USD, 0.99, Currency.THB, 34.7771, 34.43)]
    public void Should_Return_Correct_Result_When_RoundExchangeForLogic(
        Currency requestedCurrency,
        decimal inputAmount,
        Currency convertedCurrency,
        decimal exchangeRate,
        decimal expected
    )
    {
        var actual = RoundingUtils.RoundExchangeForLogic(requestedCurrency, inputAmount, convertedCurrency, exchangeRate);

        Assert.Equal(expected, actual);
    }
}