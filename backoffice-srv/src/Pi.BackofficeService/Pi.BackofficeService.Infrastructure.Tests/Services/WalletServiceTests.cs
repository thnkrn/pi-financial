using Moq;
using Pi.BackofficeService.Application.Services.WalletService;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;
using Pi.Client.WalletService.Api;
using Pi.Client.WalletService.Model;

namespace Pi.BackofficeService.Infrastructure.Tests.Services;

public class WalletServiceTests
{
    private readonly Mock<IActionApi> _actionApi;
    private readonly ITicketActionService _ticketActionService;

    public WalletServiceTests()
    {
        _actionApi = new Mock<IActionApi>();
        _ticketActionService = new Infrastructure.Services.WalletService(_actionApi.Object);
    }

    [Fact]
    [Obsolete("Obsolete")]
    public async Task Should_Return_Correctly_When_Request_Sba_Manual_Allocate()
    {
        // Arrange
        _actionApi
            .Setup(d => d.InternalWalletActionTransactionNoManualAllocateSystemPostAsync(
                It.IsAny<string>(),
                It.IsAny<PiWalletServiceAPIControllersSystem>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new PiWalletServiceAPIModelsTicketResponseApiResponse(
                    new PiWalletServiceAPIModelsTicketResponse(Guid.NewGuid())));

        // Act
        var response = await _ticketActionService.RequestSbaManualAllocationAsync("12345", new CancellationToken());

        // Assert
        Assert.True(response.Success);
    }

    [Fact]
    public async Task Should_Return_Correctly_When_Request_SetTrade_Manual_Allocate()
    {
        // Arrange
        _actionApi
            .Setup(d => d.InternalWalletActionTransactionNoManualAllocateSystemPostAsync(
                It.IsAny<string>(),
                It.IsAny<PiWalletServiceAPIControllersSystem>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new PiWalletServiceAPIModelsTicketResponseApiResponse(
                    new PiWalletServiceAPIModelsTicketResponse(Guid.NewGuid())));

        // Act
        var response =
            await _ticketActionService.RequestSetTradeManualAllocationAsync("12345", new CancellationToken());

        // Assert
        Assert.True(response.Success);
    }

    [Fact]
    public async Task Should_Return_Correctly_When_Request_Global_Manual_Allocate()
    {
        // Arrange
        _actionApi
            .Setup(d => d.InternalWalletActionTransactionNoManualAllocateSystemPostAsync(
                It.IsAny<string>(),
                It.IsAny<PiWalletServiceAPIControllersSystem>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new PiWalletServiceAPIModelsTicketResponseApiResponse(
                    new PiWalletServiceAPIModelsTicketResponse(Guid.NewGuid())));

        // Act
        var response = await _ticketActionService.RequestManualAllocationAsync("12345", new CancellationToken());

        // Assert
        Assert.True(response.Success);
    }

    [Fact]
    public async Task Should_Return_Correctly_When_Request_Refund()
    {
        // Arrange
        _actionApi
            .Setup(d => d.InternalWalletActionTransactionNoRefundPostAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                () =>
                    new PiWalletServiceAPIModelsRefundResponseApiResponse(
                        new PiWalletServiceAPIModelsRefundResponse(
                            Guid.NewGuid(),
                            "transactionNo")
                    )
            );

        // Act
        var response = await _ticketActionService.RefundTransactionAsync("12345", new CancellationToken());

        // Assert
        Assert.True(response.Success);
    }

    [Fact]
    public async Task Should_Return_Correctly_When_Request_Approve()
    {
        // Arrange
        _actionApi
            .Setup(d => d.InternalWalletActionTransactionNoApprovePostAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                () =>
                    new PiWalletServiceAPIModelsTicketResponseApiResponse(
                        new PiWalletServiceAPIModelsTicketResponse(
                            Guid.NewGuid(),
                            "transactionNo")
                    )
            );

        // Act
        var response = await _ticketActionService.ApproveDepositTransactionAsync("12345", new CancellationToken());

        // Assert
        Assert.True(response.Success);
    }

    [Fact]
    public async Task Should_Return_Correctly_When_ConfirmSba()
    {
        // Arrange
        _actionApi
            .Setup(d => d.InternalWalletActionTransactionNoConfirmSbaCallbackPostAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                () =>
                    new PiWalletServiceAPIModelsTicketResponseApiResponse(
                        new PiWalletServiceAPIModelsTicketResponse(
                            Guid.NewGuid(),
                            "transactionNo")
                    )
            );

        // Act
        var response =
            await _ticketActionService.ConfirmSbaCallbackTransactionAsync("12345", new CancellationToken());

        // Assert
        Assert.True(response.Success);
    }

    [Fact]
    public async Task Should_Return_Correctly_When_ConfirmSbaAtsDeposit()
    {
        // Arrange
        _actionApi
            .Setup(d => d.InternalWalletActionTransactionNoConfirmSbaAtsCallbackPostAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                () =>
                    new PiWalletServiceAPIModelsTicketResponseApiResponse(
                        new PiWalletServiceAPIModelsTicketResponse(
                            Guid.NewGuid(),
                            "transactionNo")
                    )
            );

        // Act
        var response =
            await _ticketActionService.ConfirmSbaAtsCallbackTransactionAsync("12345", new CancellationToken());

        // Assert
        Assert.True(response.Success);
    }

    [Fact]
    public async Task Should_Return_Correctly_When_ConfirmKkpDeposit()
    {
        // Arrange
        _actionApi
            .Setup(d => d.InternalWalletActionTransactionNoConfirmKkpCallbackPostAsync(
                It.IsAny<string>(),
                It.IsAny<PiWalletServiceAPIModelsActionConfirmKkpCallback>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                () =>
                    new PiWalletServiceAPIModelsTicketResponseApiResponse(
                        new PiWalletServiceAPIModelsTicketResponse(
                            Guid.NewGuid(),
                            "transactionNo")
                    )
            );

        // Act
        var response = await _ticketActionService.ConfirmDepositKkpCallbackTransactionAsync(
            "12345",
            100,
            DateTime.Now,
            "1234",
            "123456",
            new CancellationToken());

        // Assert
        Assert.True(response.Success);
    }

    [Fact]
    public async Task Should_Return_Correctly_When_ChangeTransactionStatus_To_Fail()
    {
        // Arrange
        _actionApi
            .Setup(d => d.InternalWalletActionTransactionNoUpdateStatusStatusPostAsync(
                It.IsAny<string>(),
                It.IsAny<PiWalletServiceDomainAggregatesModelTransactionAggregateStatus>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                () =>
                    new PiWalletServiceAPIModelsTicketResponseApiResponse(
                        new PiWalletServiceAPIModelsTicketResponse(
                            Guid.NewGuid(),
                            "transactionNo")
                    )
            );

        // Act
        var response = await _ticketActionService.ChangeTransactionStatusAsync(
            "12345",
            TransactionStatus.Fail,
            new CancellationToken());

        // Assert
        Assert.True(response.Success);
    }

    [Fact]
    public async Task Should_Return_Correctly_When_ChangeTransactionStatus_To_Pending()
    {
        // Arrange
        _actionApi
            .Setup(d => d.InternalWalletActionTransactionNoUpdateStatusStatusPostAsync(
                It.IsAny<string>(),
                It.IsAny<PiWalletServiceDomainAggregatesModelTransactionAggregateStatus>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                () =>
                    new PiWalletServiceAPIModelsTicketResponseApiResponse(
                        new PiWalletServiceAPIModelsTicketResponse(
                            Guid.NewGuid(),
                            "transactionNo")
                    )
            );

        // Act
        var response = await _ticketActionService.ChangeTransactionStatusAsync(
            "12345",
            TransactionStatus.Pending,
            new CancellationToken());

        // Assert
        Assert.True(response.Success);
    }

    [Fact]
    public async Task Should_Return_Correctly_When_ChangeTransactionStatus_To_Success()
    {
        // Arrange
        _actionApi
            .Setup(d => d.InternalWalletActionTransactionNoUpdateStatusStatusPostAsync(
                It.IsAny<string>(),
                It.IsAny<PiWalletServiceDomainAggregatesModelTransactionAggregateStatus>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                () =>
                    new PiWalletServiceAPIModelsTicketResponseApiResponse(
                        new PiWalletServiceAPIModelsTicketResponse(
                            Guid.NewGuid(),
                            "transactionNo")
                    )
            );

        // Act
        var response = await _ticketActionService.ChangeTransactionStatusAsync(
            "12345",
            TransactionStatus.Success,
            new CancellationToken());

        // Assert
        Assert.True(response.Success);
    }

    [Fact]
    public async Task Should_Return_Correctly_When_ChangeSetTradeStatus_To_Fail()
    {
        // Arrange
        _actionApi
            .Setup(d => d.InternalWalletActionTransactionNoUpdateStatusStatusPostAsync(
                It.IsAny<string>(),
                It.IsAny<PiWalletServiceDomainAggregatesModelTransactionAggregateStatus>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                () =>
                    new PiWalletServiceAPIModelsTicketResponseApiResponse(
                        new PiWalletServiceAPIModelsTicketResponse(
                            Guid.NewGuid(),
                            "transactionNo")
                    )
            );

        // Act
        var response = await _ticketActionService.ChangeSetTradeStatusAsync(
            "12345",
            TransactionStatus.Fail,
            new CancellationToken());

        // Assert
        Assert.True(response.Success);
    }

    [Fact]
    public async Task Should_Return_Correctly_When_ChangeSetTradeStatus_To_Pending()
    {
        // Arrange
        _actionApi
            .Setup(d => d.InternalWalletActionTransactionNoUpdateStatusStatusPostAsync(
                It.IsAny<string>(),
                It.IsAny<PiWalletServiceDomainAggregatesModelTransactionAggregateStatus>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                () =>
                    new PiWalletServiceAPIModelsTicketResponseApiResponse(
                        new PiWalletServiceAPIModelsTicketResponse(
                            Guid.NewGuid(),
                            "transactionNo")
                    )
            );

        // Act
        var response = await _ticketActionService.ChangeSetTradeStatusAsync(
            "12345",
            TransactionStatus.Pending,
            new CancellationToken());

        // Assert
        Assert.True(response.Success);
    }

    [Fact]
    public async Task Should_Return_Correctly_When_ChangeSetTradeStatus_To_Success()
    {
        // Arrange
        _actionApi
            .Setup(d => d.InternalWalletActionTransactionNoUpdateStatusStatusPostAsync(
                It.IsAny<string>(),
                It.IsAny<PiWalletServiceDomainAggregatesModelTransactionAggregateStatus>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                () =>
                    new PiWalletServiceAPIModelsTicketResponseApiResponse(
                        new PiWalletServiceAPIModelsTicketResponse(
                            Guid.NewGuid(),
                            "transactionNo")
                    )
            );

        // Act
        var response = await _ticketActionService.ChangeSetTradeStatusAsync(
            "12345",
            TransactionStatus.Success,
            new CancellationToken());

        // Assert
        Assert.True(response.Success);
    }

    [Fact]
    public async Task Should_Return_Correctly_When_UpdateBillPaymentReference()
    {
        // Arrange
        _actionApi
            .Setup(d => d.InternalWalletActionTransactionNoUpdateBillPaymentReferencePostAsync(
                It.IsAny<string>(),
                It.IsAny<PiWalletServiceAPIModelsActionUpdateBillPaymentReference>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                () =>
                    new PiWalletServiceAPIModelsTicketResponseApiResponse(
                        new PiWalletServiceAPIModelsTicketResponse(
                            Guid.NewGuid(),
                            "transactionNo")
                    )
            );

        // Act
        var response = await _ticketActionService.UpdateBillPaymentReferenceAsync(
            "12345",
            "123",
            new CancellationToken());

        // Assert
        Assert.True(response.Success);
    }
}