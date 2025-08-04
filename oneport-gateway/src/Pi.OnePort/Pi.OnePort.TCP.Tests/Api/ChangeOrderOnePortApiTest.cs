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

public class ChangeOnePortApiTest
{
    private Mock<IOnePortResponseMapClient> _client { get; }
    private Mock<IResponseKeyGenerator> _generator { get; }
    private OnePortApi OnePortApi { get; }

    public ChangeOnePortApiTest()
    {
        _client = new Mock<IOnePortResponseMapClient>();
        _generator = new Mock<IResponseKeyGenerator>();
        OnePortApi = new OnePortApi(_client.Object, _generator.Object, Mock.Of<ILogger<OnePortApi>>());
    }

    [Fact]
    public async Task Should_Return_7kResponse_When_ChangeOrderAsync_Success()
    {
        // Arrange
        var request = new DataTransferOrderChangeRequest7M
        {
            RefOrderId = "SomeRefOrderId",
            FisOrderId = "",
            EnterId = "",
            Account = "",
            Client = "",
            Ttf = Ttf.None,
            OrderType = OrderType.Normal,
            Price = 0,
            ConPrice = ConPrice.None,
            Volume = 0,
            PublishVol = 0,
        };
        var response = new DataTransferOrderChangeResponse7N
        {
            RefOrderId = "responseRefOrderId",
            FisOrderId = "",
            ExecutionTransType = ExecutionTransType.New,
            OrderStatus = OrderStatus.Accepted,
            ExecutionTransRejectType = null,
            Source = Source.Fis,
            Reason = "",
            EnterId = "",
            Account = "",
            Ttf = Ttf.None,
            Price = 0,
            ConPrice = ConPrice.None,
            Volume = 0,
            PublishVolume = 0
        };
        _generator.Setup(q => q.NewKey(It.IsAny<Packet>()))
            .Returns("someKey");
        _client.Setup(q => q.SendAndWaitResponse<PacketDataTransfer>(It.IsAny<string>(), It.IsAny<Packet>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PacketDataTransfer(response));

        // Act
        var actual = await OnePortApi.ChangeOrderAsync(request);

        // Assert
        Assert.IsType<DataTransferOrderChangeResponse7N>(actual);
        Assert.Equal(response, actual);
    }

    [Fact]
    public async Task Should_Error_When_ChangeOrderAsync_Without_ResponseKey()
    {
        // Arrange
        var request = new DataTransferOrderChangeRequest7M
        {
            RefOrderId = "SomeRefOrderId",
            FisOrderId = "",
            EnterId = "",
            Account = "",
            Client = "",
            Ttf = Ttf.None,
            OrderType = OrderType.Normal,
            Price = 0,
            ConPrice = ConPrice.None,
            Volume = 0,
            PublishVol = 0,
        };
        _generator.Setup(q => q.NewKey(It.IsAny<Packet>()))
            .Returns((string?)null);

        // Act
        var act = async () => await OnePortApi.ChangeOrderAsync(request);

        // Assert
        await Assert.ThrowsAsync<OnePortApiException>(act);
    }

    [Fact]
    public async Task Should_Error_When_ChangeOrderAsync_And_TimeOut()
    {
        // Arrange
        var request = new DataTransferOrderChangeRequest7M
        {
            RefOrderId = "SomeRefOrderId",
            FisOrderId = "",
            EnterId = "",
            Account = "",
            Client = "",
            Ttf = Ttf.None,
            OrderType = OrderType.Normal,
            Price = 0,
            ConPrice = ConPrice.None,
            Volume = 0,
            PublishVol = 0,
        };
        _generator.Setup(q => q.NewKey(It.IsAny<Packet>()))
            .Returns("someKey");
        _client.Setup(q =>
                q.SendAndWaitResponse<PacketDataTransfer>(It.IsAny<string>(), It.IsAny<Packet>(),
                    It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TimeoutException());

        // Act
        var act = async () => await OnePortApi.ChangeOrderAsync(request);

        // Assert
        await Assert.ThrowsAsync<OnePortApiException>(act);
    }

    [Fact]
    public async Task Should_Error_When_ChangeOrderAsync_And_ResponseNotExpected()
    {
        // Arrange
        var request = new DataTransferOrderChangeRequest7M
        {
            RefOrderId = "SomeRefOrderId",
            FisOrderId = "",
            EnterId = "",
            Account = "",
            Client = "",
            Ttf = Ttf.None,
            OrderType = OrderType.Normal,
            Price = 0,
            ConPrice = ConPrice.None,
            Volume = 0,
            PublishVol = 0,
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
        var act = async () => await OnePortApi.ChangeOrderAsync(request);

        // Assert
        await Assert.ThrowsAsync<OnePortApiException>(act);
    }
}
