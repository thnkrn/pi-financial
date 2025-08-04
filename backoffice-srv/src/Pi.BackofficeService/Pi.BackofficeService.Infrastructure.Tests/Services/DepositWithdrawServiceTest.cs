using Moq;
using Pi.BackofficeService.Application.Services.WalletService;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using Pi.Client.WalletService.Api;
using Pi.Client.WalletService.Model;
using Product = Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate.Product;

namespace Pi.BackofficeService.Infrastructure.Tests.Services;

public class DepositWithdrawServiceTest
{
    private readonly Mock<ITransactionApi> _transactionApiMock;
    private readonly IDepositWithdrawService _depositWithdrawService;

    public DepositWithdrawServiceTest()
    {
        _transactionApiMock = new Mock<ITransactionApi>();
        _depositWithdrawService = new Infrastructure.Services.DepositWithdrawService(_transactionApiMock.Object);
    }

    [Theory]
    [InlineData(Channel.SetTrade, PiWalletServiceIntegrationEventsAggregatesModelChannel.SetTrade)]
    [InlineData(Channel.QR, PiWalletServiceIntegrationEventsAggregatesModelChannel.QR)]
    [InlineData(Channel.ODD, PiWalletServiceIntegrationEventsAggregatesModelChannel.ODD)]
    [InlineData(Channel.BillPayment, PiWalletServiceIntegrationEventsAggregatesModelChannel.BillPayment)]
    public async Task GetTransactionHistoriesV2_ShouldReturnCorrectResponse(Channel channel, PiWalletServiceIntegrationEventsAggregatesModelChannel walletChannel)
    {
        var expected = GetExpectedTransactionHistoriesV2Response(walletChannel);

        _transactionApiMock.Setup(t => t.InternalTransactionsGetAsync(
            It.IsAny<string?>(),
            It.IsAny<List<PiWalletServiceIntegrationEventsAggregatesModelProduct>?>(),
            It.IsAny<PiWalletServiceDomainAggregatesModelTransactionAggregateStatus?>(),
            It.IsAny<string?>(),
            It.IsAny<PiWalletServiceIntegrationEventsAggregatesModelTransactionType?>(),
            It.IsAny<PiWalletServiceIntegrationEventsAggregatesModelChannel?>(),
            It.IsAny<DateTime?>(),
            It.IsAny<DateTime?>(),
            It.IsAny<DateTime?>(),
            It.IsAny<DateTime?>(),
            It.IsAny<DateTime?>(),
            It.IsAny<DateTime?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<List<string>>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(expected);

        var filter = new TransactionHistoryV2Filter(
            null,
            null,
            null,
            null,
            null,
            channel,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null
        );

        var filterWithProductList = new TransactionHistoryV2Filter(
            new List<Product> { Product.Funds, Product.TFEX, Product.Cash, Product.Crypto, Product.CashBalance, Product.Margin, Product.GlobalEquity },
            null,
            null,
            null,
            null,
            Channel.AtsBatch,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null
        );

        var response =
            await _depositWithdrawService.GetTransactionHistoriesV2(filter);
        var responseWithProductList =
            await _depositWithdrawService.GetTransactionHistoriesV2(filterWithProductList);

        Assert.NotNull(response);
        Assert.Equal(expected.Data.First().State, response.Data.First().State);
        Assert.Equal(expected.Data.First().Product.ToString(), response.Data.First().Product.ToString());
        Assert.Equal(expected.Data.First().AccountCode, response.Data.First().AccountCode);
        Assert.Equal(expected.Data.First().CustomerName, response.Data.First().CustomerName);
        Assert.Equal(expected.Data.First().BankAccountNo, response.Data.First().BankAccountNo);
        Assert.Equal(expected.Data.First().BankAccountName, response.Data.First().BankAccountName);
        Assert.Equal(expected.Data.First().BankName, response.Data.First().BankName);
        Assert.Equal(DateOnly.FromDateTime(expected.Data.First().EffectiveDate!.Value), response.Data.First().EffectiveDate);
        Assert.Equal(expected.Data.First().GlobalAccount, response.Data.First().GlobalAccount);
        Assert.Equal(expected.Data.First().TransactionNo, response.Data.First().TransactionNo);
        Assert.Equal(expected.Data.First().TransactionType.ToString(), response.Data.First().TransactionType.ToString());
        Assert.Equal(expected.Data.First().RequestedAmount, response.Data.First().RequestedAmount);
        Assert.Equal(expected.Data.First().RequestedCurrency.ToString(), response.Data.First().RequestedCurrency.ToString());
        Assert.Equal(expected.Data.First().Status, response.Data.First().Status);
        Assert.Equal(expected.Data.First().CreatedAt, response.Data.First().CreatedAt);
        Assert.Equal(expected.Data.First().ToCurrency.ToString(), response.Data.First().ToCurrency.ToString());
        Assert.Equal(expected.Data.First().TransferAmount, response.Data.First().TransferAmount);
        Assert.Equal(expected.Data.First().Channel.ToString(), response.Data.First().Channel.ToString());
        Assert.Equal(expected.Data.First().BankAccount, response.Data.First().BankAccount);
        Assert.Equal(expected.Data.First().Fee, response.Data.First().Fee);
        Assert.Equal(expected.Data.First().TransferFee, response.Data.First().TransferFee);

        Assert.NotNull(responseWithProductList);
        Assert.Equal(expected.Data.First().AccountCode, responseWithProductList.Data.First().AccountCode);
        Assert.Equal(expected.Data.First().CustomerName, responseWithProductList.Data.First().CustomerName);
        Assert.Equal(expected.Data.First().BankAccount, responseWithProductList.Data.First().BankAccount);
        Assert.Equal(expected.Data.First().BankAccountNo, responseWithProductList.Data.First().BankAccountNo);
        Assert.Equal(expected.Data.First().BankAccountName, responseWithProductList.Data.First().BankAccountName);
    }

    [Fact]
    public async Task GetTransactionV2ByTransactionNo_ShouldReturnCorrectResponse()
    {
        // Arrange
        var transactionNo = "transactionNo";
        var transactionDto = new PiWalletServiceAPIModelsTransactionDetailsDtoApiResponse()
        {
            Data = new PiWalletServiceAPIModelsTransactionDetailsDto()
            {
                CurrentState = "currentState",
                Status = PiWalletServiceDomainAggregatesModelTransactionAggregateStatus.Pending,
                Product = PiWalletServiceIntegrationEventsAggregatesModelProduct.CashBalance,
                Channel = PiWalletServiceIntegrationEventsAggregatesModelChannel.ATS,
                TransactionType = PiWalletServiceIntegrationEventsAggregatesModelTransactionType.Deposit,
                TransactionNo = transactionNo,
            }
        };
        _transactionApiMock.Setup(api => api.InternalTransactionTransactionNoGetAsync(transactionNo, "Initial", It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactionDto);

        // Act
        var actual = await _depositWithdrawService.GetTransactionV2ByTransactionNo(transactionNo);

        // Assert
        Assert.NotNull(actual);
        Assert.IsType<TransactionV2>(actual);
        Assert.Equal(transactionDto.Data.CurrentState, actual.State);
        Assert.Equal(transactionDto.Data.Status.ToString(), actual.Status);
        Assert.Equal(transactionDto.Data.TransactionNo, actual.TransactionNo);
        Assert.Equal(transactionDto.Data.Product.ToString(), actual.Product.ToString());
        Assert.Equal(Channel.AtsBatch, actual.Channel);
        // ... assert other properties as needed
    }

    private static PiWalletServiceAPIModelsTransactionHistoryV2ListApiPaginateResponse GetExpectedTransactionHistoriesV2Response(PiWalletServiceIntegrationEventsAggregatesModelChannel channel)
    {
        return new PiWalletServiceAPIModelsTransactionHistoryV2ListApiPaginateResponse(
            new List<PiWalletServiceAPIModelsTransactionHistoryV2>
            {
                new (
                    "Final",
                    PiWalletServiceIntegrationEventsAggregatesModelProduct.Cash,
                    "123456",
                    "John Doe",
                    "11223344567",
                    "John Doe",
                    "The Siam Commercial Bank Public Company Limited",
                    DateTime.Now,
                    DateTime.Now,
                    "",
                    "",
                    "DHDP2000010100001",
                    PiWalletServiceIntegrationEventsAggregatesModelTransactionType.Deposit,
                    "20.00",
                    PiWalletServiceDomainAggregatesModelGlobalWalletAggregateCurrency.THB,
                    "",
                    DateTime.Now,
                    PiWalletServiceDomainAggregatesModelGlobalWalletAggregateCurrency.USD,
                    "20.00",
                    channel,
                    "The Siam Commercial Bank Public Company Limited • 0099"
                    )
            }, 1, 20, 20);
    }
}
