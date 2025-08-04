using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Pi.WalletService.Application.Commands.Deposit;
using Pi.WalletService.Application.Options;
using Pi.WalletService.Application.Services.Bank;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.QrDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Domain.Events.Deposit;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Tests.Commands.Deposit;

public class GenerateDepositQrRequestTests : ConsumerTest
{
    private readonly Mock<IBankService> _bankService;
    private readonly Mock<IQrDepositRepository> _qrDepositRepository;
    private readonly Mock<IOptionsSnapshot<FeeOptions>> _options;

    public GenerateDepositQrRequestTests()
    {
        Mock<IDepositEntrypointRepository> depositEntrypointRepository = new Mock<IDepositEntrypointRepository>();
        Mock<IWithdrawEntrypointRepository> withdrawEntrypointRepository = new Mock<IWithdrawEntrypointRepository>();
        _qrDepositRepository = new Mock<IQrDepositRepository>();
        _bankService = new Mock<IBankService>();
        _options = new Mock<IOptionsSnapshot<FeeOptions>>();
        _options.Setup(o => o.Value).Returns(new FeeOptions
        {
            KKP = new Kkp
            {
                DepositFee = "0"
            }
        });
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<GenerateQrConsumer>(); })
            .AddScoped<IBankService>(_ => _bankService.Object)
            .AddScoped<IDepositEntrypointRepository>(_ => depositEntrypointRepository.Object)
            .AddScoped<IWithdrawEntrypointRepository>(_ => withdrawEntrypointRepository.Object)
            .AddScoped<IQrDepositRepository>(_ => _qrDepositRepository.Object)
            .AddScoped<ILogger<GenerateQrConsumer>>(_ => NullLogger<GenerateQrConsumer>.Instance)
            .AddScoped<IOptionsSnapshot<FeeOptions>>(_ => _options.Object)
            .BuildServiceProvider(true);
    }

    [Fact]
    public async Task Should_Able_To_Generate_QR_Successful()
    {
        // Arrange
        const string qrValue = "qrValue";
        _bankService.Setup(b => b.GenerateQR(
                It.IsAny<string>(),
                It.IsAny<decimal>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>()))
            .ReturnsAsync(
                new QRPaymentResponse(
                    null,
                    null,
                    string.Empty,
                    string.Empty,
                    true,
                    qrValue
                    ));

        var client = Harness.GetRequestClient<GenerateDepositQrRequest>();

        // Act
        var response = await client.GetResponse<QrCodeGenerated>(
            new GenerateDepositQrRequest(
                Guid.NewGuid(),
                2_000_000,
                "7711431",
                Product.Cash,
                90,
                "transaction_no"
                ));

        // Assert
        Assert.Equal(qrValue, response.Message.QrValue);
    }

    [Theory]
    [InlineData(0, "0")]
    [InlineData(2_000_001, "0")]
    [InlineData(5, "10")]
    public async Task Should_Throw_AmountExceedAllowedAmountException_When_Amount_Is_More_Or_Less_Than_Accepted(decimal amount, string fee)
    {
        // Arrange
        _bankService.Setup(b => b.GenerateQR(
                It.IsAny<string>(),
                It.IsAny<decimal>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>()))
            .ReturnsAsync(
                new QRPaymentResponse(
                    null,
                    null,
                    string.Empty,
                    string.Empty,
                    true,
                    string.Empty
                ));
        _options.Setup(o => o.Value).Returns(new FeeOptions
        {
            KKP = new Kkp
            {
                DepositFee = fee
            }
        });
        var client = Harness.GetRequestClient<GenerateDepositQrRequest>();

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () =>
            await client.GetResponse<QrCodeGenerated>(
            new GenerateDepositQrRequest(
                Guid.NewGuid(),
                amount,
                "7711431",
                Product.Cash,
                90,
                "transaction_no"
            )));

        // Assert
        Assert.True(exception.Fault?.Exceptions.Any(e => e.ExceptionType.Equals(typeof(AmountExceedAllowedAmountException).ToString())));
    }

    [Fact]
    public async Task Should_Throw_UnableToGenerateQrException_When_Unable_To_Generate_Qr()
    {
        // Arrange
        _bankService.Setup(b => b.GenerateQR(
                It.IsAny<string>(),
                It.IsAny<decimal>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>()))
            .ThrowsAsync(new Exception("Something went wrong"));

        var client = Harness.GetRequestClient<GenerateDepositQrRequest>();

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () =>
            await client.GetResponse<QrCodeGenerated>(
                new GenerateDepositQrRequest(
                    Guid.NewGuid(),
                    200,
                    "7711431",
                    Product.Cash,
                    90,
                    "transaction_no"
                )));

        // Assert
        Assert.True(exception.Fault?.Exceptions.Any(e => e.ExceptionType.Equals(typeof(UnableToGenerateQrException).ToString())));
    }
}
