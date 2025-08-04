using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pi.Common.Features;
using Pi.WalletService.Application.Commands.GlobalWalletTransfer;
using Pi.WalletService.Application.Services.FxService;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.Events.ForeignExchange;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Tests.Commands.GlobalWalletTransfer;

public class RequestFxTransactionTests : ConsumerTest
{
    private readonly Mock<IConfiguration> _config;
    private readonly Mock<IFxService> _fxService;
    private readonly Mock<IFeatureService> _featureService;
    private readonly GetTransactionResponse _fxResponse;

    private const string FxRateExpireTime = "15";
    public RequestFxTransactionTests()
    {
        _fxService = new Mock<IFxService>();
        _config = new Mock<IConfiguration>();
        _config.Setup(x => x["Fx:FxRateExpireTime"]).Returns(FxRateExpireTime);
        _featureService = new Mock<IFeatureService>();

        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<RequestFxTransactionConsumer>(); })
            .AddScoped<IFxService>(_ => _fxService.Object)
            .AddScoped<IConfiguration>(_ => _config.Object)
            .AddScoped<IFeatureService>(_ => _featureService.Object)
            .BuildServiceProvider(true);
        _fxResponse = new GetTransactionResponse(
            Guid.NewGuid().ToString(),
            FxQuoteType.Sell,
            DateTime.Now,
            2000,
            Currency.USD.ToString(),
            200,
            Currency.THB.ToString(),
            decimal.Parse("24.5"),
            DateTime.Now.Subtract(
                TimeSpan.FromMinutes(double.Round(int.Parse(FxRateExpireTime) / 2))
            )
        );
    }

    [Fact]
    public async void Should_Able_To_Get_Fx_Info_Correctly()
    {
        // Arrange
        _fxService
            .Setup(fx => fx.GetTransaction(It.IsAny<string>()))
            .ReturnsAsync(_fxResponse);
        var client = Harness.GetRequestClient<RequestFxTransaction>();

        // Act
        var response = await client.GetResponse<QueryFxTransactionSucceed>(
            new RequestFxTransaction(
                // Guid.NewGuid().ToString(),
                TransactionType.Withdraw,
                _fxResponse.Id,
                DateTime.Now,
                0
            )
        );

        // Assert
        Assert.Equal(_fxResponse.Id, response.Message.Id);
        Assert.Equal(_fxResponse.ValueDate, response.Message.ValueDate);
        Assert.Equal(_fxResponse.ContractAmount, response.Message.ContractAmount);
        Assert.Equal(_fxResponse.ContractCurrency, response.Message.ContractCurrency.ToString());
        Assert.Equal(_fxResponse.ExchangeRate, response.Message.ExchangeRate);
        Assert.Equal(_fxResponse.TransactionDateTime, response.Message.TransactionDateTime);
    }

    [Theory]
    [InlineData(FxQuoteType.Buy, TransactionType.Withdraw)]
    [InlineData(FxQuoteType.Sell, TransactionType.Deposit)]
    public async void Should_Handle_Get_Fx_Info_Correctly_When_QuoteType_And_TransactionType_Mismatch(FxQuoteType quoteType, TransactionType transactionType)
    {
        // Arrange
        _fxService
            .Setup(fx => fx.GetTransaction(It.IsAny<string>()))
            .ReturnsAsync(
                new GetTransactionResponse(
                    Guid.NewGuid().ToString(),
                    quoteType,
                    DateTime.Now,
                    2000,
                    "HKD",
                    200,
                    Currency.THB.ToString(),
                    decimal.Parse("24.5"),
                    DateTime.Now
                ));
        var client = Harness.GetRequestClient<RequestFxTransaction>();

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () =>
            await client.GetResponse<QueryFxTransactionSucceed>(
                new RequestFxTransaction(
                    // Guid.NewGuid().ToString(),
                    transactionType,
                    _fxResponse.Id,
                    DateTime.Now,
                    0
                )));

        // Assert
        Assert.True(exception.Fault?.Exceptions.Any(e => e.ExceptionType.Equals(typeof(InvalidDataException).ToString())));
    }

    [Fact]
    public async void Should_Handle_Get_Fx_Info_Correctly_When_Contract_Currency_Not_Supported()
    {
        // Arrange
        _fxService
            .Setup(fx => fx.GetTransaction(It.IsAny<string>()))
            .ReturnsAsync(new GetTransactionResponse(
                Guid.NewGuid().ToString(),
                FxQuoteType.Buy,
                DateTime.Now,
                2000,
                "HKD",
                200,
                Currency.THB.ToString(),
                decimal.Parse("24.5"),
                DateTime.Now
            ));

        var client = Harness.GetRequestClient<RequestFxTransaction>();

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () =>
            await client.GetResponse<QueryFxTransactionSucceed>(
                new RequestFxTransaction(
                    // Guid.NewGuid().ToString(),
                    TransactionType.Deposit,
                    _fxResponse.Id,
                    DateTime.Now,
                    0
                )));

        // Assert
        Assert.True(exception.Fault?.Exceptions.Any(e => e.ExceptionType.Equals(typeof(InvalidDataException).ToString())));
    }

    [Fact]
    public async void Should_Handle_Get_Fx_Info_Correctly_When_Counter_Currency_Not_Supported()
    {
        // Arrange
        _fxService
            .Setup(fx => fx.GetTransaction(It.IsAny<string>()))
            .ReturnsAsync(new GetTransactionResponse(
                Guid.NewGuid().ToString(),
                FxQuoteType.Sell,
                DateTime.Now,
                2000,
                Currency.USD.ToString(),
                200,
                "HKD",
                decimal.Parse("24.5"),
                DateTime.Now
            ));
        var client = Harness.GetRequestClient<RequestFxTransaction>();

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () =>
            await client.GetResponse<QueryFxTransactionSucceed>(
                new RequestFxTransaction(
                    // Guid.NewGuid().ToString(),
                    TransactionType.Withdraw,
                    _fxResponse.Id,
                    DateTime.Now,
                    0
                )));

        // Assert
        Assert.True(exception.Fault?.Exceptions.Any(e => e.ExceptionType.Equals(typeof(InvalidDataException).ToString())));
    }

    [Fact]
    public async void Should_Handle_Get_Fx_Info_Correctly_When_Fx_Transaction_Has_Expired()
    {
        // Arrange
        _fxService
            .Setup(fx => fx.GetTransaction(It.IsAny<string>()))
            .ReturnsAsync(new GetTransactionResponse(
                Guid.NewGuid().ToString(),
                FxQuoteType.Sell,
                DateTime.Now,
                2000,
                Currency.USD.ToString(),
                200,
                "HKD",
                decimal.Parse("24.5"),
                DateTime.Now.Subtract(TimeSpan.FromMinutes(int.Parse(FxRateExpireTime)))
            ));
        var client = Harness.GetRequestClient<RequestFxTransaction>();

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () =>
            await client.GetResponse<QueryFxTransactionSucceed>(
                new RequestFxTransaction(
                    // Guid.NewGuid().ToString(),
                    TransactionType.Withdraw,
                    _fxResponse.Id,
                    DateTime.Now,
                    0
                )));

        // Assert
        Assert.True(exception.Fault?.Exceptions.Any(e => e.ExceptionType.Equals(typeof(InvalidDataException).ToString())));
    }
}
