using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pi.SetService.Application.Commands;
using Pi.SetService.Application.Exceptions;
using Pi.SetService.Domain.AggregatesModel.ErrorAggregate;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.SetService.Application.Tests.Commands;

public class ReviewSblOrderConsumerTest : ConsumerTest
{
    private readonly Mock<ISblOrderRepository> _sblOrderRepository;

    public ReviewSblOrderConsumerTest()
    {
        _sblOrderRepository = new Mock<ISblOrderRepository>();
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<ReviewSblOrderConsumer>(); })
            .AddScoped<ISblOrderRepository>(_ => _sblOrderRepository.Object)
            .BuildServiceProvider(true);
    }

    [Theory]
    [InlineData(SblOrderStatus.Approved, null, SblOrderStatus.Approved, null)]
    [InlineData(SblOrderStatus.Approved, "Something", SblOrderStatus.Approved, null)]
    [InlineData(SblOrderStatus.Rejected, null, SblOrderStatus.Rejected, null)]
    [InlineData(SblOrderStatus.Rejected, "Something", SblOrderStatus.Rejected, "Something")]
    public async Task Should_ReturnExpectedResponse_When_Success(SblOrderStatus status, string? rejectedReason,
        SblOrderStatus expectedStatus, string? expectedRejectReason)
    {
        // Arrange
        var client = Harness.GetRequestClient<ReviewSblOrder>();
        var order = new SblOrder(Guid.NewGuid(), Guid.NewGuid(), "0803174-6", "0803174", 1, "EA",
            SblOrderStatus.Pending, 100, SblOrderType.Borrow, null, null);
        var message = new ReviewSblOrder
        {
            Id = order.Id,
            Status = status,
            ReviewerId = Guid.NewGuid(),
            RejectedReason = rejectedReason
        };

        _sblOrderRepository.Setup(repo => repo.GetSblOrder(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _sblOrderRepository.Setup(q => q.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var response = await client.GetResponse<ReviewSblOrderResponse>(message);

        // Assert
        Assert.IsType<ReviewSblOrderResponse>(response.Message);
        Assert.Equal(message.Id, response.Message.Id);
        Assert.Equal(expectedStatus, response.Message.Status);
        Assert.Equal(message.ReviewerId, response.Message.ReviewerId);
        Assert.Equal(expectedRejectReason, response.Message.RejectedReason);
    }

    [Theory]
    [InlineData(SblOrderStatus.Approved, null, SblOrderStatus.Approved, null)]
    [InlineData(SblOrderStatus.Approved, "Something", SblOrderStatus.Approved, null)]
    [InlineData(SblOrderStatus.Rejected, null, SblOrderStatus.Rejected, null)]
    [InlineData(SblOrderStatus.Rejected, "Something", SblOrderStatus.Rejected, "Something")]
    public async Task Should_UpdateSblOrderToApproved_When_Success(SblOrderStatus status, string? rejectedReason,
        SblOrderStatus expectedStatus, string? expectedRejectReason)
    {
        // Arrange
        var client = Harness.GetRequestClient<ReviewSblOrder>();
        var order = new SblOrder(Guid.NewGuid(), Guid.NewGuid(), "0803174-6", "0803174", 1, "EA",
            SblOrderStatus.Pending, 100, SblOrderType.Borrow, null, null);
        var message = new ReviewSblOrder
        {
            Id = order.Id,
            Status = status,
            ReviewerId = Guid.NewGuid(),
            RejectedReason = rejectedReason
        };

        _sblOrderRepository.Setup(repo => repo.GetSblOrder(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _sblOrderRepository.Setup(q => q.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await client.GetResponse<ReviewSblOrderResponse>(message);

        // Assert
        _sblOrderRepository.Verify(repo => repo.Update(It.Is<SblOrder>(o => o.Status == expectedStatus)), Times.Once);
        _sblOrderRepository.Verify(repo => repo.Update(It.Is<SblOrder>(o => o.RejectedReason == expectedRejectReason)),
            Times.Once);
        _sblOrderRepository.Verify(repo => repo.Update(It.Is<SblOrder>(o => o.ReviewerId == message.ReviewerId)),
            Times.Once);
        _sblOrderRepository.Verify(repo => repo.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Error_When_ReviewWithPendingStatus()
    {
        // Arrange
        var client = Harness.GetRequestClient<ReviewSblOrder>();
        var order = new SblOrder(Guid.NewGuid(), Guid.NewGuid(), "0803174-6", "0803174", 1, "EA",
            SblOrderStatus.Pending, 100, SblOrderType.Borrow, null, null);
        var message = new ReviewSblOrder
        {
            Id = order.Id,
            Status = SblOrderStatus.Pending,
            ReviewerId = Guid.NewGuid()
        };

        // Act
        var act = async () => await client.GetResponse<ReviewSblOrderResponse>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(SetException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(SetException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE003.ToString(), code);
    }

    [Theory]
    [InlineData(SblOrderStatus.Approved)]
    [InlineData(SblOrderStatus.Rejected)]
    public async Task Should_Error_When_SblOrderNotFound(SblOrderStatus requestStatus)
    {
        // Arrange
        var client = Harness.GetRequestClient<ReviewSblOrder>();
        var message = new ReviewSblOrder
        {
            Id = Guid.NewGuid(),
            Status = requestStatus,
            ReviewerId = Guid.NewGuid()
        };

        _sblOrderRepository.Setup(repo => repo.GetSblOrder(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SblOrder?)null);

        // Act
        var act = async () => await client.GetResponse<ReviewSblOrderResponse>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(SetException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(SetException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE116.ToString(), code);
    }

    [Theory]
    [InlineData(SblOrderStatus.Approved)]
    [InlineData(SblOrderStatus.Rejected)]
    public async Task Should_Error_When_SblOrderIsNotPendingStatus(SblOrderStatus orderStatus)
    {
        // Arrange
        var client = Harness.GetRequestClient<ReviewSblOrder>();
        var order = new SblOrder(Guid.NewGuid(), Guid.NewGuid(), "0803174-6", "0803174", 1, "EA",
            orderStatus, 100, SblOrderType.Borrow, null, null);
        var message = new ReviewSblOrder
        {
            Id = order.Id,
            Status = SblOrderStatus.Approved,
            ReviewerId = Guid.NewGuid()
        };

        _sblOrderRepository.Setup(repo => repo.GetSblOrder(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var act = async () => await client.GetResponse<ReviewSblOrderResponse>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(SetException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(SetException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE116.ToString(), code);
    }
}
