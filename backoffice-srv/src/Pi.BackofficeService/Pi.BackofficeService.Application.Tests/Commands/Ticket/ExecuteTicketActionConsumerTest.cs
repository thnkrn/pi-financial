using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pi.BackofficeService.Application.Commands.Ticket;
using Pi.BackofficeService.Application.Services.WalletService;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;

namespace Pi.BackofficeService.Application.Tests.Commands.Ticket;

public class ExecuteTicketActionConsumerTest : ConsumerTest
{
    private readonly Mock<ITicketActionService> _walletService;

    public ExecuteTicketActionConsumerTest()
    {
        _walletService = new Mock<ITicketActionService>();
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<ExecuteTicketActionConsumer>(); })
            .AddScoped<ITicketActionService>(_ => _walletService.Object)
            .BuildServiceProvider(true);
    }

    [Fact]
    public async void Should_Return_Success_When_Execute_Approve_Method()
    {
        // Arrange
        var client = Harness.GetRequestClient<ExecuteTicketActionMessage>();
        var payload = new ExecuteTicketActionMessage("DD123", "CustCode", Method.Approve, null);
        _walletService.Setup(q => q.ApproveDepositTransactionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExecuteTicketActionResponse(true));

        // Act
        var response = await client.GetResponse<ExecuteTicketActionResponse>(payload);

        // Assert
        Assert.True(response.Message.Success);
    }

    [Fact]
    public async void Should_Throw_Exception_When_Execute_Approve_Method_And_Service_ThrowException()
    {
        // Arrange
        var client = Harness.GetRequestClient<ExecuteTicketActionMessage>();
        var payload = new ExecuteTicketActionMessage("DD123", "CustCode", Method.Approve, null);
        _walletService.Setup(q => q.RequestManualAllocationAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Some Exception"));

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () =>
        {
            await client.GetResponse<ExecuteTicketActionResponse>(payload);
        });

        // Assert
        Assert.True(exception.Fault?.Exceptions.Any());
    }

    [Fact]
    public async void Should_Return_Success_When_Execute_Refund_Method()
    {
        // Arrange
        var client = Harness.GetRequestClient<ExecuteTicketActionMessage>();
        var payload = new ExecuteTicketActionMessage("DD123", "CustCode", Method.Refund, null);
        _walletService.Setup(q => q.RefundTransactionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExecuteTicketActionResponse(true));

        // Act
        var response = await client.GetResponse<ExecuteTicketActionResponse>(payload);

        // Assert
        Assert.True(response.Message.Success);
    }

    [Fact]
    public async void Should_Throw_TicketExecuteException_When_Execute_Refund_Method_And_Service_ThrowException()
    {
        // Arrange
        var client = Harness.GetRequestClient<ExecuteTicketActionMessage>();
        var payload = new ExecuteTicketActionMessage("DD123", "CustCode", Method.Refund, null);
        _walletService.Setup(q => q.RefundTransactionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Some Exception"));

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () =>
        {
            await client.GetResponse<ExecuteTicketActionResponse>(payload);
        });

        // Assert
        Assert.True(exception.Fault?.Exceptions.Any());
    }

    [Fact]
    public async void Should_Return_Success_When_Execute_CcyAllocationTransfer_Method()
    {
        // Arrange
        var client = Harness.GetRequestClient<ExecuteTicketActionMessage>();
        var payload = new ExecuteTicketActionMessage("DD123", "CustCode", Method.CcyAllocationTransfer, null);
        _walletService.Setup(q => q.RequestManualAllocationAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExecuteTicketActionResponse(true));

        // Act
        var response = await client.GetResponse<ExecuteTicketActionResponse>(payload);

        // Assert
        Assert.True(response.Message.Success);
    }

    [Fact]
    public async void Should_Throw_Exception_When_Execute_CcyAllocationTransfer_Method_And_Service_ThrowException()
    {
        // Arrange
        var client = Harness.GetRequestClient<ExecuteTicketActionMessage>();
        var payload = new ExecuteTicketActionMessage("DD123", "CustCode", Method.CcyAllocationTransfer, null);
        _walletService.Setup(q => q.RequestManualAllocationAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Some Exception"));

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () =>
        {
            await client.GetResponse<ExecuteTicketActionResponse>(payload);
        });

        // Assert
        Assert.True(exception.Fault?.Exceptions.Any());
    }

    [Fact]
    public async void Should_Return_Success_When_Execute_SbaManualAllocate_Method()
    {
        // Arrange
        var client = Harness.GetRequestClient<ExecuteTicketActionMessage>();
        var payload = new ExecuteTicketActionMessage("DD123", "CustCode", Method.RetrySbaDeposit, null);
        _walletService
            .Setup(q => q.RequestSbaManualAllocationAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExecuteTicketActionResponse(true));

        // Act
        var response = await client.GetResponse<ExecuteTicketActionResponse>(payload);

        // Assert
        Assert.True(response.Message.Success);
    }

    [Fact]
    public async void Should_Return_Success_When_Execute_SetTradeManualAllocate_Method()
    {
        // Arrange
        var client = Harness.GetRequestClient<ExecuteTicketActionMessage>();
        var payload = new ExecuteTicketActionMessage("DD123", "CustCode", Method.RetrySetTradeDeposit, null);
        _walletService
            .Setup(q => q.RequestSetTradeManualAllocationAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExecuteTicketActionResponse(true));

        // Act
        var response = await client.GetResponse<ExecuteTicketActionResponse>(payload);

        // Assert
        Assert.True(response.Message.Success);
    }

    [Fact]
    public async void Should_Return_Success_When_Execute_SbaConfirm_Method()
    {
        // Arrange
        var client = Harness.GetRequestClient<ExecuteTicketActionMessage>();
        var payload = new ExecuteTicketActionMessage("DD123", "CustCode", Method.SbaConfirm, null);
        _walletService.Setup(q =>
                q.ConfirmSbaCallbackTransactionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExecuteTicketActionResponse(true));

        // Act
        var response = await client.GetResponse<ExecuteTicketActionResponse>(payload);

        // Assert
        Assert.True(response.Message.Success);
    }

    [Fact]
    public async void Should_Return_Success_When_Execute_SbaDepositAtsCallbackConfirm_Method()
    {
        // Arrange
        var client = Harness.GetRequestClient<ExecuteTicketActionMessage>();
        var payload = new ExecuteTicketActionMessage("DD123", "CustCode", Method.SbaDepositAtsCallbackConfirm, null);
        _walletService.Setup(q =>
                q.ConfirmSbaAtsCallbackTransactionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExecuteTicketActionResponse(true));

        // Act
        var response = await client.GetResponse<ExecuteTicketActionResponse>(payload);

        // Assert
        Assert.True(response.Message.Success);
    }

    [Fact]
    public async void Should_Return_Success_When_Execute_DepositKkpConfirm_Method()
    {
        // Arrange
        var client = Harness.GetRequestClient<ExecuteTicketActionMessage>();
        var payload = new ExecuteTicketActionMessage(
            "DD123",
            "CustCode",
            Method.DepositKkpConfirm,
            "{\"paymentReceivedAmount\":20.00,\"paymentReceivedDateTime\":\"2024-03-26T04:36:42.819Z\",\"bankCode\":\"014\",\"bankAccountNo\":\"123456\"}");

        _walletService.Setup(q => q.ConfirmDepositKkpCallbackTransactionAsync(
                It.IsAny<string>(),
                It.IsAny<decimal>(),
                It.IsAny<DateTime>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExecuteTicketActionResponse(true));

        // Act
        var response = await client.GetResponse<ExecuteTicketActionResponse>(payload);

        // Assert
        Assert.True(response.Message.Success);
    }

    [Fact]
    public async void Should_ThrowException_When_Execute_DepositKkpConfirm_Method()
    {
        // Arrange
        var client = Harness.GetRequestClient<ExecuteTicketActionMessage>();
        var payload = new ExecuteTicketActionMessage(
            "DD123",
            "CustCode",
            Method.DepositKkpConfirm,
            "");

        _walletService.Setup(q => q.ConfirmDepositKkpCallbackTransactionAsync(
                It.IsAny<string>(),
                It.IsAny<decimal>(),
                It.IsAny<DateTime>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExecuteTicketActionResponse(true));

        // Act && Assert
        var response = await Assert.ThrowsAsync<RequestFaultException>(async () =>
            await client.GetResponse<ExecuteTicketActionResponse>(payload));

        Assert.Equal("System.IO.InvalidDataException", response.Fault?.Exceptions.FirstOrDefault()?.ExceptionType);
    }

    [Fact]
    public async void Should_Return_Success_When_Execute_ChangeTransactionStatusToFail_Method()
    {
        // Arrange
        var client = Harness.GetRequestClient<ExecuteTicketActionMessage>();
        var payload = new ExecuteTicketActionMessage("DD123", "CustCode", Method.ChangeStatusToFail, null);
        _walletService.Setup(q =>
                q.ChangeTransactionStatusAsync(It.IsAny<string>(), TransactionStatus.Fail, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExecuteTicketActionResponse(true));

        // Act
        var response = await client.GetResponse<ExecuteTicketActionResponse>(payload);

        // Assert
        Assert.True(response.Message.Success);
    }

    [Fact]
    public async void Should_Return_Success_When_Execute_ChangeTransactionStatusToPending_Method()
    {
        // Arrange
        var client = Harness.GetRequestClient<ExecuteTicketActionMessage>();
        var payload = new ExecuteTicketActionMessage("DD123", "CustCode", Method.ChangeStatusToPending, null);
        _walletService.Setup(q =>
                q.ChangeTransactionStatusAsync(It.IsAny<string>(), TransactionStatus.Pending, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExecuteTicketActionResponse(true));

        // Act
        var response = await client.GetResponse<ExecuteTicketActionResponse>(payload);

        // Assert
        Assert.True(response.Message.Success);
    }

    [Fact]
    public async void Should_Return_Success_When_Execute_ChangeTransactionStatusToSuccess_Method()
    {
        // Arrange
        var client = Harness.GetRequestClient<ExecuteTicketActionMessage>();
        var payload = new ExecuteTicketActionMessage("DD123", "CustCode", Method.ChangeStatusToSuccess, null);
        _walletService.Setup(q =>
                q.ChangeTransactionStatusAsync(It.IsAny<string>(), TransactionStatus.Success, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExecuteTicketActionResponse(true));

        // Act
        var response = await client.GetResponse<ExecuteTicketActionResponse>(payload);

        // Assert
        Assert.True(response.Message.Success);
    }

    [Fact]
    public async void Should_Return_Success_When_Execute_ChangeSetTradeStatusToFail_Method()
    {
        // Arrange
        var client = Harness.GetRequestClient<ExecuteTicketActionMessage>();
        var payload = new ExecuteTicketActionMessage("DD123", "CustCode", Method.ChangeSetTradeStatusToFail, null);
        _walletService.Setup(q =>
                q.ChangeSetTradeStatusAsync(It.IsAny<string>(), TransactionStatus.Fail, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExecuteTicketActionResponse(true));

        // Act
        var response = await client.GetResponse<ExecuteTicketActionResponse>(payload);

        // Assert
        Assert.True(response.Message.Success);
    }

    [Fact]
    public async void Should_Return_Success_When_Execute_ChangeSetTradeStatusToPending_Method()
    {
        // Arrange
        var client = Harness.GetRequestClient<ExecuteTicketActionMessage>();
        var payload = new ExecuteTicketActionMessage("DD123", "CustCode", Method.ChangeSetTradeStatusToPending, null);
        _walletService.Setup(q =>
                q.ChangeSetTradeStatusAsync(It.IsAny<string>(), TransactionStatus.Pending, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExecuteTicketActionResponse(true));

        // Act
        var response = await client.GetResponse<ExecuteTicketActionResponse>(payload);

        // Assert
        Assert.True(response.Message.Success);
    }

    [Fact]
    public async void Should_Return_Success_When_Execute_ChangeSetTradeStatusToSuccess_Method()
    {
        // Arrange
        var client = Harness.GetRequestClient<ExecuteTicketActionMessage>();
        var payload = new ExecuteTicketActionMessage("DD123", "CustCode", Method.ChangeSetTradeStatusToSuccess, null);
        _walletService.Setup(q =>
                q.ChangeSetTradeStatusAsync(It.IsAny<string>(), TransactionStatus.Success, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExecuteTicketActionResponse(true));

        // Act
        var response = await client.GetResponse<ExecuteTicketActionResponse>(payload);

        // Assert
        Assert.True(response.Message.Success);
    }

    [Fact]
    public async void Should_Return_Success_When_Execute_UpdateBillPaymentReferenceAction_Method()
    {
        // Arrange
        var client = Harness.GetRequestClient<ExecuteTicketActionMessage>();
        var payload = new ExecuteTicketActionMessage("DD123", "CustCode", Method.UpdateBillPaymentReference, "{\"oldReference\":\"oldRef\",\"newReference\":\"newRef\"}");
        _walletService.Setup(q =>
                q.UpdateBillPaymentReferenceAsync(It.IsAny<string>(), "newRef", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExecuteTicketActionResponse(true));

        // Act
        var response = await client.GetResponse<ExecuteTicketActionResponse>(payload);

        // Assert
        Assert.True(response.Message.Success);
    }

    [Fact]
    public async void Should_ThrowException_When_Execute_UpdateBillPaymentReferenceAction_Method()
    {
        // Arrange
        var client = Harness.GetRequestClient<ExecuteTicketActionMessage>();
        var payload = new ExecuteTicketActionMessage(
            "DD123",
            "CustCode",
            Method.UpdateBillPaymentReference,
            "");

        _walletService.Setup(q => q.UpdateBillPaymentReferenceAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExecuteTicketActionResponse(false));

        // Act && Assert
        var response = await Assert.ThrowsAsync<RequestFaultException>(async () =>
            await client.GetResponse<ExecuteTicketActionResponse>(payload));

        Assert.Equal("System.IO.InvalidDataException", response.Fault?.Exceptions.FirstOrDefault()?.ExceptionType);
    }
}