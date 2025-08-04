using Pi.OnePort.TCP.Enums.DataTransfer;
using Pi.OnePort.TCP.Generators;
using Pi.OnePort.TCP.Models;
using Pi.OnePort.TCP.Models.Packets;
using Pi.OnePort.TCP.Models.Packets.DataTransfer;

namespace Pi.OnePort.TCP.Tests.Generators;

public class ResponseKeyGeneratorTest
{
    [Theory]
    [MemberData(nameof(GetTestData))]
    public void Should_Return_Expected_When_NewKey(Packet packet, string? keyExpected)
    {
        // Arrange
        var generator = new ResponseKeyGenerator();

        // Act
        var actual = generator.NewKey(packet);

        // Assert
        Assert.Equal(keyExpected, actual);
    }

    public static IEnumerable<object[]> GetTestData()
    {
        return new List<object?[]>
        {
            new object?[]
            {
                new Packet(
                    new PacketLogon
                    {
                        LoginId = "",
                        Password = ""
                    }),
                null
            },
            new object[]
            {
                new Packet(
                    new PacketDataTransfer(
                        new DataTransferOrderAcknowledgementResponse7K
                        {
                            RefOrderId = "RefTestPackage7K",
                            FisOrderId = "0000034384",
                            ExecutionTransType = ExecutionTransType.New,
                            OrderStatus = OrderStatus.Accepted,
                            ExecutionTransRejectType = null,
                            Source = Source.Fis,
                            Reason = "",
                        })
                ),
                "7a000000000000000000000000000000000000000000000000RefTestPackage7K"
            },
            new object[]
            {
                new Packet(
                    new PacketDataTransfer(
                        new DataTransferOrderAcknowledgementResponse7K
                        {
                            RefOrderId = "RefTestPackage7K",
                            FisOrderId = "0000034384",
                            ExecutionTransType = ExecutionTransType.Cancel,
                            OrderStatus = OrderStatus.Accepted,
                            ExecutionTransRejectType = null,
                            Source = Source.Fis,
                            Reason = "",
                        })
                ),
                "7c000000000000000000000000000000000000000000000000RefTestPackage7K"
            },
            new object?[]
            {
                new Packet(
                    new PacketDataTransfer(
                        new DataTransferOrderAcknowledgementResponse7K
                        {
                            RefOrderId = "RefTestPackage7K",
                            FisOrderId = "0000034384",
                            ExecutionTransType = ExecutionTransType.ChangeAcct,
                            OrderStatus = OrderStatus.Accepted,
                            ExecutionTransRejectType = null,
                            Source = Source.Fis,
                            Reason = "",
                        })
                ),
                null
            },
            new object[]
            {
                new Packet(
                    new PacketDataTransfer(
                        new DataTransferOrderChangeResponse7N()
                        {
                            RefOrderId = "RefTestPackage7N",
                            FisOrderId = "0000034384",
                            EnterId = "9004",
                            Account = "01669771",
                            ExecutionTransType = ExecutionTransType.ChangeAcct,
                            Ttf = Ttf.None,
                            Price = 0,
                            ConPrice = ConPrice.None,
                            Volume = 0,
                            PublishVolume = 0,
                            OrderStatus = OrderStatus.Rejected,
                            ExecutionTransRejectType = ExecutionTransRejectType.Set,
                            Source = Source.Set,
                            Reason = "The price may not be improved in this session state"
                        })
                ),
                "7m000000000000000000000000000000000000000000000000RefTestPackage7N"
            },
            new object[]
            {
                new Packet(
                    new PacketDataTransfer(
                        new DataTransferOrderCancelRequest7C()
                        {
                            RefOrderId = "RefTestPackage7C",
                            FisOrderId = "0000034384",
                            EnterId = "9004",
                        })
                ),
                "7c000000000000000000000000000000000000000000000000RefTestPackage7C"
            },
            new object[]
            {
                new Packet(
                    new PacketDataTransfer(
                        new DataTransferNewOrderRequest7A
                        {
                            RefOrderId = "RefTestPackage7A",
                            EnterId = "9004",
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
                            ValidTillData = 0,
                        })
                ),
                "7a000000000000000000000000000000000000000000000000RefTestPackage7A"
            },
            new object[]
            {
                new Packet(
                    new PacketDataTransfer(
                        new DataTransferOrderChangeRequest7M
                        {
                            RefOrderId = "RefTestPackage7M",
                            FisOrderId = "0000034384",
                            EnterId = "9004",
                            Account = null,
                            Client = null,
                            Ttf = Ttf.None,
                            OrderType = OrderType.Normal,
                            Price = 0,
                            ConPrice = ConPrice.None,
                            Volume = 0,
                            PublishVol = 0,
                        })
                ),
                "7m000000000000000000000000000000000000000000000000RefTestPackage7M"
            },
        };
    }
}
