using Microsoft.Extensions.Logging;
using Moq;
using Pi.OnePort.TCP.Api;
using Pi.OnePort.TCP.Client;
using Pi.OnePort.TCP.Enums.DataTransfer;
using Pi.OnePort.TCP.Exceptions;
using Pi.OnePort.TCP.Generators;
using Pi.OnePort.TCP.Models;
using Pi.OnePort.TCP.Models.Packets;
using Pi.OnePort.TCP.Models.Packets.DataTransfer;

namespace Pi.OnePort.TCP.Tests.Api;

public class NewOrderOnePortApiTest
{
    private Mock<IOnePortResponseMapClient> _client { get; }
    private Mock<IResponseKeyGenerator> _generator { get; }
    private OnePortApi OnePortApi { get; }

    public NewOrderOnePortApiTest()
    {
        _client = new Mock<IOnePortResponseMapClient>();
        _generator = new Mock<IResponseKeyGenerator>();
        OnePortApi = new OnePortApi(_client.Object, _generator.Object, Mock.Of<ILogger<OnePortApi>>());
    }

    [Fact]
    public async Task Should_Return_ExpectedResponse_When_NewOrderAsync_Success()
    {
        // Arrange
        var request = new DataTransferNewOrderRequest7A
        {
            RefOrderId = "SomeRefOrderId",
            EnterId = "",
            SecSymbol = "",
            Side = OrderSide.Buy,
            Price = null,
            ConPrice = ConPrice.None,
            Volume = 0,
            PublishVol = 0,
            Condition = Conditions.None,
            Account = "",
            Ttf = Ttf.None,
            OrderType = OrderType.Normal,
            CheckFlag = "",
            ValidTillData = 0
        };
        var response = new DataTransferOrderAcknowledgementResponse7K
        {
            RefOrderId = "responseRefOrderId",
            FisOrderId = "",
            ExecutionTransType = ExecutionTransType.New,
            OrderStatus = OrderStatus.Accepted,
            ExecutionTransRejectType = null,
            Source = Source.Fis,
            Reason = ""
        };
        _generator.Setup(q => q.NewKey(It.IsAny<Packet>()))
            .Returns("someKey");
        _client.Setup(q => q.SendAndWaitResponse<PacketDataTransfer>(It.IsAny<string>(), It.IsAny<Packet>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PacketDataTransfer(response));

        // Act
        var actual = await OnePortApi.NewOrderAsync(request);

        // Assert
        Assert.IsType<DataTransferOrderAcknowledgementResponse7K>(actual);
        Assert.Equal(response, actual);
    }

    [Fact]
    public async Task Should_Error_When_NewOrderAsync_Without_ResponseKey()
    {
        // Arrange
        var request = new DataTransferNewOrderRequest7A
        {
            RefOrderId = "SomeRefOrderId",
            EnterId = "",
            SecSymbol = "",
            Side = OrderSide.Buy,
            Price = null,
            ConPrice = ConPrice.None,
            Volume = 0,
            PublishVol = 0,
            Condition = Conditions.None,
            Account = "",
            Ttf = Ttf.None,
            OrderType = OrderType.Normal,
            CheckFlag = "",
            ValidTillData = 0
        };
        _generator.Setup(q => q.NewKey(It.IsAny<Packet>()))
            .Returns((string?)null);

        // Act
        var act = async () => await OnePortApi.NewOrderAsync(request);

        // Assert
        await Assert.ThrowsAsync<OnePortApiException>(act);
    }

    [Fact]
    public async Task Should_Error_When_NewOrderAsync_And_TimeOut()
    {
        // Arrange
        var request = new DataTransferNewOrderRequest7A
        {
            RefOrderId = "SomeRefOrderId",
            EnterId = "",
            SecSymbol = "",
            Side = OrderSide.Buy,
            Price = null,
            ConPrice = ConPrice.None,
            Volume = 0,
            PublishVol = 0,
            Condition = Conditions.None,
            Account = "",
            Ttf = Ttf.None,
            OrderType = OrderType.Normal,
            CheckFlag = "",
            ValidTillData = 0
        };
        _generator.Setup(q => q.NewKey(It.IsAny<Packet>()))
            .Returns("someKey");
        _client.Setup(q =>
                q.SendAndWaitResponse<PacketDataTransfer>(It.IsAny<string>(), It.IsAny<Packet>(),
                    It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TimeoutException());

        // Act
        var act = async () => await OnePortApi.NewOrderAsync(request);

        // Assert
        await Assert.ThrowsAsync<OnePortApiException>(act);
    }

    [Fact]
    public async Task Should_Error_When_NewOrderAsync_And_ResponseNotExpected()
    {
        // Arrange
        var request = new DataTransferNewOrderRequest7A
        {
            RefOrderId = "SomeRefOrderId",
            EnterId = "",
            SecSymbol = "",
            Side = OrderSide.Buy,
            Price = null,
            ConPrice = ConPrice.None,
            Volume = 0,
            PublishVol = 0,
            Condition = Conditions.None,
            Account = "",
            Ttf = Ttf.None,
            OrderType = OrderType.Normal,
            CheckFlag = "",
            ValidTillData = 0
        };
        var response = new DataTransferExecutionReportResponse7E
        {
            RefOrderId = "SomeRefOrderId7E",
            FisOrderId = "",
            ExecutionTransType = ExecutionTransType.New,
            TransTime = default,
            SecSymbol = "",
            Side = OrderSide.Buy,
            Volume = 0,
            Price = 0,
            ConfirmNo = null,
            Source = Source.Fis,
            ExecType = ExecType.Normal,
        };
        _generator.Setup(q => q.NewKey(It.IsAny<Packet>()))
            .Returns("someKey");
        _client.Setup(q =>
                q.SendAndWaitResponse<PacketDataTransfer>(It.IsAny<string>(), It.IsAny<Packet>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PacketDataTransfer(response));

        // Act
        var act = async () => await OnePortApi.NewOrderAsync(request);

        // Assert
        await Assert.ThrowsAsync<OnePortApiException>(act);
    }
}
