using Moq;
using Pi.BackofficeService.Application.Services.WalletService;
using Pi.BackofficeService.Domain.AggregateModels.TransferCashAggregate;
using Pi.BackofficeService.Infrastructure.Services;
using Pi.Client.WalletService.Api;
using Pi.Client.WalletService.Client;
using Pi.Client.WalletService.Model;

namespace Pi.BackofficeService.Infrastructure.Tests.Services;

public class TransferCashTests
{
    private readonly Mock<ITransferApi> _transferCashApi;
    private readonly TransferCashService _transferCashService;

    public TransferCashTests()
    {
        _transferCashApi = new Mock<ITransferApi>();
        _transferCashService = new TransferCashService(_transferCashApi.Object);
    }

    [Fact]
    public async Task GetTransferCashByTransactionNo_ShouldReturnCorrectResponse()
    {
        // Arrange
        var transactionNo = "transactionNo";
        var transactionDto = new PiWalletServiceAPIModelsTransferCashDtoApiResponse()
        {
            Data = new PiWalletServiceAPIModelsTransferCashDto()
            {
                CorrelationId = Guid.NewGuid(),
                State = "currentState",
                Status = PiWalletServiceDomainAggregatesModelTransactionAggregateStatus.Pending,
                TransferFromExchangeMarket = PiWalletServiceIntegrationEventsAggregatesModelProduct.Cash,
                TransferToExchangeMarket = PiWalletServiceIntegrationEventsAggregatesModelProduct.CashBalance,
                TransferFromAccountCode = "transferFrom",
                TransferToAccountCode = "transferTo",
                TransactionNo = transactionNo,
            }
        };
        _transferCashApi.Setup(api => api.InternalTransferCashTransactionNoGetAsync(transactionNo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactionDto);

        // Act
        var actual = await _transferCashService.GetTransferCashByTransactionNo(transactionNo, new CancellationToken());

        // Assert
        Assert.NotNull(actual);
        Assert.IsType<TransferCash>(actual);
        Assert.Equal(transactionDto.Data.CorrelationId, actual.Id);
        Assert.Equal(transactionDto.Data.Status.ToString(), actual.Status);
        Assert.Equal(transactionDto.Data.TransactionNo, actual.TransactionNo);
        Assert.Equal(transactionDto.Data.TransferFromExchangeMarket.ToString(), actual.TransferFromExchangeMarket.ToString());
        Assert.Equal(transactionDto.Data.TransferToExchangeMarket.ToString(), actual.TransferToExchangeMarket.ToString());
    }

    [Fact]
    public async Task GetTransferCashByTransactionNo_ShouldReturnNullWhenGotException()
    {
        _transferCashApi.Setup(api => api.InternalTransferCashTransactionNoGetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ApiException());

        var actual = await _transferCashService.GetTransferCashByTransactionNo("transactionNo", new CancellationToken());

        Assert.Null(actual);
    }

    [Fact]
    public async Task GetTransferCashHistory_ShouldReturnCorrectResponse()
    {
        var transferCash = new PiWalletServiceAPIModelsTransferCashDto
        {
            CorrelationId = Guid.NewGuid(),
            State = "currentState",
            Status = PiWalletServiceDomainAggregatesModelTransactionAggregateStatus.Pending,
            TransferFromExchangeMarket = PiWalletServiceIntegrationEventsAggregatesModelProduct.Cash,
            TransferToExchangeMarket = PiWalletServiceIntegrationEventsAggregatesModelProduct.CashBalance,
            TransferFromAccountCode = "transferFrom",
            TransferToAccountCode = "transferTo",
            TransactionNo = "transactionNo",
        };
        var transferCashList = new PiWalletServiceAPIModelsTransferCashDtoListApiPaginateResponse([transferCash], 1, 20, 1);
        _transferCashApi.Setup(api => api.InternalTransferCashGetAsync(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transferCashList);
        var transferCashTransactionFilter = new TransferCashTransactionFilter(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);

        var actual = await _transferCashService.GetTransferCashHistory(transferCashTransactionFilter, new CancellationToken());

        Assert.NotNull(actual);
        Assert.IsType<PaginateResponse<TransferCash>>(actual);
        Assert.Equal(transferCash.CorrelationId, actual.Data.First().Id);
        Assert.Equal(transferCash.State, actual.Data.First().State);
        Assert.Equal(transferCash.Status.ToString(), actual.Data.First().Status);
        Assert.Equal(transferCash.TransferFromExchangeMarket.ToString(), actual.Data.First().TransferFromExchangeMarket.ToString());
        Assert.Equal(transferCash.TransferToExchangeMarket.ToString(), actual.Data.First().TransferToExchangeMarket.ToString());
        Assert.Equal(transferCash.TransferFromAccountCode, actual.Data.First().TransferFromAccountCode);
        Assert.Equal(transferCash.TransferToAccountCode, actual.Data.First().TransferToAccountCode);
        Assert.Equal(transferCash.TransactionNo, actual.Data.First().TransactionNo);
    }

    [Fact]
    public async Task GetTransferCashHistory_ShouldReturnNullWhenGotException()
    {
        _transferCashApi.Setup(api => api.InternalTransferCashGetAsync(null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ApiException());
        var transferCashTransactionFilter = new TransferCashTransactionFilter(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);

        var actual = await _transferCashService.GetTransferCashHistory(transferCashTransactionFilter, new CancellationToken());

        Assert.Null(actual);
    }
}
