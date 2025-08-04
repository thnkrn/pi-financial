using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Pi.Common.Features;
using Pi.WalletService.Application.Commands;
using Pi.WalletService.Application.Options;
using Pi.WalletService.Domain.AggregatesModel.CashAggregate;
using Pi.WalletService.Domain.AggregatesModel.DepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Domain.Events;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Tests;

public class GenerateTransactionNoConsumerTests : ConsumerTest
{
    private readonly Mock<IDepositRepository> _depositRepository;
    private readonly Mock<IGlobalWalletDepositRepository> _globalWalletDepositRepository;
    private readonly Mock<ICashWithdrawRepository> _cashWithdrawRepository;
    private readonly Mock<IDepositEntrypointRepository> _depositEntrypointRepository;
    private readonly Mock<IWithdrawEntrypointRepository> _withdrawEntrypointRepository;
    private readonly Mock<IOptionsSnapshot<TransactionNoCutOffTimeOptions>> _transactionNoCutOffTimeOptions;
    private readonly Mock<IFeatureService> _featureService;

    public GenerateTransactionNoConsumerTests()
    {
        _depositRepository = new Mock<IDepositRepository>();
        _globalWalletDepositRepository = new Mock<IGlobalWalletDepositRepository>();
        _cashWithdrawRepository = new Mock<ICashWithdrawRepository>();
        _depositEntrypointRepository = new Mock<IDepositEntrypointRepository>();
        _withdrawEntrypointRepository = new Mock<IWithdrawEntrypointRepository>();
        _transactionNoCutOffTimeOptions = new Mock<IOptionsSnapshot<TransactionNoCutOffTimeOptions>>();
        _featureService = new Mock<IFeatureService>();

        _transactionNoCutOffTimeOptions.Setup(q => q.Value).Returns(new TransactionNoCutOffTimeOptions
        {
            NonGe = "00:00",
            Ge = "05:00"
        });

        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<GenerateTransactionNoConsumer>(); })
            .AddScoped<IDepositRepository>(_ => _depositRepository.Object)
            .AddScoped<IGlobalWalletDepositRepository>(_ => _globalWalletDepositRepository.Object)
            .AddScoped<ICashWithdrawRepository>(_ => _cashWithdrawRepository.Object)
            .AddScoped<IDepositEntrypointRepository>(_ => _depositEntrypointRepository.Object)
            .AddScoped<IWithdrawEntrypointRepository>(_ => _withdrawEntrypointRepository.Object)
            .AddScoped<IFeatureService>(_ => _featureService.Object)
            .AddScoped<ILogger<GenerateTransactionNoConsumer>>(_ => NullLogger<GenerateTransactionNoConsumer>.Instance)
            .BuildServiceProvider(true);
    }

    [Theory]
    [InlineData(Product.Cash, "DH", Channel.QR, TransactionType.Deposit)]
    [InlineData(Product.Cash, "DH", Channel.QR, TransactionType.Withdraw)]
    [InlineData(Product.CashBalance, "DH", Channel.QR, TransactionType.Deposit)]
    [InlineData(Product.CashBalance, "DH", Channel.QR, TransactionType.Withdraw)]
    [InlineData(Product.CreditBalance, "DH", Channel.QR, TransactionType.Deposit)]
    [InlineData(Product.CreditBalance, "DH", Channel.QR, TransactionType.Withdraw)]
    [InlineData(Product.CreditBalanceSbl, "DH", Channel.QR, TransactionType.Deposit)]
    [InlineData(Product.CreditBalanceSbl, "DH", Channel.QR, TransactionType.Withdraw)]
    [InlineData(Product.Derivatives, "DH", Channel.QR, TransactionType.Deposit)]
    [InlineData(Product.Derivatives, "DH", Channel.QR, TransactionType.Withdraw)]
    [InlineData(Product.GlobalEquities, "GE", Channel.QR, TransactionType.Deposit)]
    [InlineData(Product.GlobalEquities, "GE", Channel.QR, TransactionType.Withdraw)]
    public async void Should_Generate_Transaction_No_For_Supported_Product_Correctly(Product product, string productInitial, Channel channel, TransactionType transactionType)
    {
        // Arrange
        _globalWalletDepositRepository
            .Setup(d => d.CountTransactionNoByDate(It.IsAny<DateOnly>(), It.IsAny<TimeOnly>(), It.IsAny<bool>(), productInitial, It.IsAny<Channel>(), It.IsAny<TransactionType>()))
            .ReturnsAsync(5);
        _depositRepository
            .Setup(d => d.CountTransactionNoByDate(It.IsAny<DateOnly>(), It.IsAny<TimeOnly>(), It.IsAny<bool>(), productInitial, It.IsAny<Channel>(), It.IsAny<TransactionType>()))
            .ReturnsAsync(5);
        _cashWithdrawRepository
            .Setup(d => d.CountTransactionNoByDate(It.IsAny<DateOnly>(), It.IsAny<TimeOnly>(), It.IsAny<bool>(), productInitial, It.IsAny<Channel>(), It.IsAny<TransactionType>()))
            .ReturnsAsync(5);
        var prefix = transactionType == TransactionType.Deposit
            ? "DP"
            : "WS";
        var client = Harness.GetRequestClient<GenerateTransactionNo>();

        // Act
        var response = await client.GetResponse<TransactionNoGenerated>(
            new GenerateTransactionNo(
                Guid.NewGuid(),
                product,
                channel,
                transactionType,
                false
            )
        );

        Assert.Equal($"{productInitial}{prefix}{DateTime.UtcNow.AddHours(7):yyyyMMdd}00006", response.Message.TransactionNo);
    }

    [Theory]
    [InlineData(Product.Cash, "DH", Channel.QR, TransactionType.Deposit)]
    [InlineData(Product.Cash, "OD", Channel.ODD, TransactionType.Deposit)]
    [InlineData(Product.Cash, "AS", Channel.ATS, TransactionType.Deposit)]
    [InlineData(Product.Cash, "DH", Channel.QR, TransactionType.Withdraw)]
    [InlineData(Product.CashBalance, "DH", Channel.QR, TransactionType.Deposit)]
    [InlineData(Product.CashBalance, "DH", Channel.QR, TransactionType.Withdraw)]
    [InlineData(Product.CreditBalance, "DH", Channel.QR, TransactionType.Deposit)]
    [InlineData(Product.CreditBalance, "DH", Channel.QR, TransactionType.Withdraw)]
    [InlineData(Product.CreditBalanceSbl, "DH", Channel.QR, TransactionType.Deposit)]
    [InlineData(Product.CreditBalanceSbl, "DH", Channel.QR, TransactionType.Withdraw)]
    [InlineData(Product.Derivatives, "DH", Channel.QR, TransactionType.Deposit)]
    [InlineData(Product.Derivatives, "DH", Channel.QR, TransactionType.Withdraw)]
    [InlineData(Product.GlobalEquities, "GE", Channel.QR, TransactionType.Deposit)]
    [InlineData(Product.GlobalEquities, "GE", Channel.QR, TransactionType.Withdraw)]
    [InlineData(Product.GlobalEquities, "GA", Channel.ATS, TransactionType.Deposit)]
    [InlineData(Product.GlobalEquities, "GO", Channel.ODD, TransactionType.Deposit)]
    public async void Should_Generate_Transaction_No_For_Supported_Product_Correctly_V2(Product product, string productInitial, Channel channel, TransactionType transactionType)
    {
        // Arrange
        if (transactionType == TransactionType.Deposit)
        {
            _depositEntrypointRepository
                .Setup(d => d.CountTransactionNoByDate(It.IsAny<DateOnly>(), It.IsAny<TimeOnly>(), It.IsAny<bool>(),
                    productInitial, It.IsAny<Channel>(), It.IsAny<TransactionType>()))
                .ReturnsAsync(1);
            if (product == Product.GlobalEquities)
            {
                _globalWalletDepositRepository
                    .Setup(d => d.CountTransactionNoByDate(It.IsAny<DateOnly>(), It.IsAny<TimeOnly>(), It.IsAny<bool>(), productInitial, It.IsAny<Channel>(), It.IsAny<TransactionType>()))
                    .ReturnsAsync(1);
            }
            else
            {
                _depositRepository
                    .Setup(d => d.CountTransactionNoByDate(It.IsAny<DateOnly>(), It.IsAny<TimeOnly>(), It.IsAny<bool>(), productInitial, It.IsAny<Channel>(), It.IsAny<TransactionType>()))
                    .ReturnsAsync(1);
            }
        }
        else
        {
            _withdrawEntrypointRepository
                .Setup(d => d.CountTransactionNoByDate(It.IsAny<DateOnly>(), It.IsAny<TimeOnly>(), It.IsAny<bool>(),
                    productInitial, It.IsAny<Channel>(), It.IsAny<TransactionType>()))
                .ReturnsAsync(1);
            if (product == Product.GlobalEquities)
            {
                _globalWalletDepositRepository
                    .Setup(d => d.CountTransactionNoByDate(It.IsAny<DateOnly>(), It.IsAny<TimeOnly>(), It.IsAny<bool>(), productInitial, It.IsAny<Channel>(), It.IsAny<TransactionType>()))
                    .ReturnsAsync(1);
            }
            else
            {
                _cashWithdrawRepository
                    .Setup(d => d.CountTransactionNoByDate(It.IsAny<DateOnly>(), It.IsAny<TimeOnly>(), It.IsAny<bool>(), productInitial, It.IsAny<Channel>(), It.IsAny<TransactionType>()))
                    .ReturnsAsync(1);
            }
        }

        var prefix = transactionType == TransactionType.Deposit
            ? "DP"
            : "WS";
        var client = Harness.GetRequestClient<GenerateTransactionNo>();

        // Act
        var response = await client.GetResponse<TransactionNoGenerated>(
            new GenerateTransactionNo(
                Guid.NewGuid(),
                product,
                channel,
                transactionType,
                true
            )
        );

        Assert.Equal($"{productInitial}{prefix}{DateTime.UtcNow.AddHours(7):yyyyMMdd}00003", response.Message.TransactionNo);
    }

    [Theory]
    [InlineData(Product.Crypto, Channel.QR)]
    [InlineData(Product.Funds, Channel.QR)]
    public async void Should_Handle_Generate_TransactionNo_Correctly_When_Product_Not_Supported(Product product, Channel channel)
    {
        // Arrange
        var client = Harness.GetRequestClient<GenerateTransactionNo>();

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () =>
            await client.GetResponse<TransactionNoGenerated>(
            new GenerateTransactionNo(
                Guid.NewGuid(),
                product,
                channel,
                TransactionType.Deposit,
                false
            )
        ));

        Assert.True(exception.Fault?.Exceptions.Any(e => e.ExceptionType.Equals(typeof(NotImplementedException).ToString())));
    }

    [Fact]
    public async void Should_Handle_Generate_TransactionNo_Correctly_When_Product_Invalid()
    {
        // Arrange
        var client = Harness.GetRequestClient<GenerateTransactionNo>();

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () =>
            await client.GetResponse<TransactionNoGenerated>(
                new GenerateTransactionNo(
                    Guid.NewGuid(),
                    (Product)999,
                    Channel.QR,
                    TransactionType.Deposit,
                    false
                )
            ));

        Assert.True(exception.Fault?.Exceptions.Any(e => e.ExceptionType.Equals(typeof(ArgumentOutOfRangeException).ToString())));
    }

    [Fact]
    public async void Should_Handle_Generate_TransactionNo_Correctly_When_Try_To_Update_Existing_TransactionNo()
    {
        _globalWalletDepositRepository
            .SetupSequence(g => g.UpdateTransactionNo(It.IsAny<Guid>(), It.IsAny<string>()))
            .ThrowsAsync(new DuplicateTransactionNoException())
            .ThrowsAsync(new Exception());

        var client = Harness.GetRequestClient<GenerateTransactionNo>();

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () =>
            await client.GetResponse<TransactionNoGenerated>(
                new GenerateTransactionNo(
                    Guid.NewGuid(),
                    Product.GlobalEquities,
                    Channel.QR,
                    TransactionType.Deposit,
                    false
                )
            ));

        Assert.True(exception.Fault?.Exceptions.Any(e => e.ExceptionType.Equals(typeof(Exception).ToString())));
        _globalWalletDepositRepository.Verify(g => g.UpdateTransactionNo(It.IsAny<Guid>(), It.IsAny<string>()), Times.Exactly(2));
    }
}
