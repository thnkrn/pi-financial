using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Pi.Common.Features;
using Pi.WalletService.Application.Options;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Application.Services.FxService;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Tests.Queries;

public class ExchangeQueriesTest
{
    private readonly Mock<ILogger<ExchangeQueries>> _loggerMock;
    private readonly Mock<IFxService> _fxServiceMock;
    private readonly Mock<IFeatureService> _featureService;
    private readonly IExchangeQueries _exchangeQueries;
    private readonly Mock<IOptions<FxOptions>> _fxOptionsMock;

    public ExchangeQueriesTest()
    {
        _loggerMock = new Mock<ILogger<ExchangeQueries>>();
        _fxServiceMock = new Mock<IFxService>();
        _featureService = new Mock<IFeatureService>();
        _fxOptionsMock = new Mock<IOptions<FxOptions>>();
        Mock<IConfiguration> configuration = new();
        configuration.Setup(c => c["Fx:AccountTHB"]).Returns("THB-1");
        configuration.Setup(c => c["Fx:AccountUSD"]).Returns("USD-1");
        _exchangeQueries = new ExchangeQueries(_loggerMock.Object, _fxServiceMock.Object, _featureService.Object, configuration.Object, _fxOptionsMock.Object);
    }

    [Fact]
    public Task ExchangeQueries_ExchangeCurrency_ShouldReturnCorrectResponse()
    {
        var response = _exchangeQueries.ExchangeCurrency(TransactionType.Deposit, "THB", 350, "USD", 35);

        Assert.Equal(10, response);
        return Task.CompletedTask;
    }

    [Fact]
    public Task ExchangeQueries_ExchangeCurrency_ShouldThrowExceptionWhenInputInvalidCurrency()
    {
        Assert.Throws<InvalidDataException>(() => _exchangeQueries.ExchangeCurrency(TransactionType.Deposit, "EUR", 350, "USD", 35));
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(FxQuoteType.Buy, "THB", 350, "USD", "Elton John")]
    [InlineData(FxQuoteType.Sell, "THB", 350, "USD", "Elton John")]
    public async Task ExchangeQueries_GetExchangeRate_ShouldReturnCorrectResponse(FxQuoteType quoteType, string contractCurrency, decimal contractAmount, string counterCurrency, string requestedBy)
    {
        var initiateResponse = new InitiateResponse(
            "",
            10,
            350,
            Currency.USD,
            Currency.THB,
            35,
            DateTime.Now.AddDays(1));

        _fxServiceMock.Setup(f => f.Initiate(It.IsAny<InitiateRequest>())).ReturnsAsync(initiateResponse);

        var response = await _exchangeQueries.GetExchangeRate(quoteType, contractCurrency, contractAmount, counterCurrency, requestedBy);

        Assert.Equal(10, response.CounterAmount);
        Assert.Equal(Currency.USD.ToString(), response.Currency);
        Assert.Equal(35, response.Rate);
    }

    [Fact(Skip = "Flaky")]
    public async Task ExchangeQueries_GetExchangeRate_ShouldThrowExceptionWhenInputInvalidFxQuoteType()
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _exchangeQueries.GetExchangeRate((FxQuoteType)123, "THB", 350, "USD", "Elton John"));
    }
}