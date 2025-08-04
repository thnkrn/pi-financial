using Pi.OnePort.TCP.Enums.DataTransfer;
using Pi.OnePort.TCP.Models;
using Pi.OnePort.TCP.Models.Packets;
using Pi.OnePort.TCP.Models.Packets.DataTransfer;

namespace Pi.OnePort.TCP.Tests.PacketDeserialization;

public class DataTransferOrderChangeByBrokerResponse6Test
{
    
    public static IEnumerable<object[]> GetTestData()
    {
        return new List<object[]>
        {
            new object[]
            {
                "00000853DT20241218-1505256t000000000000000000000000000000000000000000000000000000000000000000000217899004       01711608          33.25000 100          100",
                new Packet(
                    new PacketDataTransfer(
                        new DataTransferOrderChangeByBrokerResponse6T
                        {
                            RefOrderId = "0000000000000000000000000000000000000000000000000000000000000000",
                            FisOrderId = "0000021789",
                            EnterId = "9004",
                            Account = "01711608",
                            PortClient = "",
                            Ttf = Ttf.None,
                            OrderType = OrderType.Normal,
                            Price = 33.25000m,
                            ConPrice = ConPrice.None,
                            Volume = 100,
                            PublishVolume = 100
                        })
                )
                {
                    Sequence = 853,
                    Timestamp = DateTime.Parse("2024-12-18T15:05:25.0000000")
                }
            }
        };
    }

    [Theory]
    [MemberData(nameof(GetTestData))]
    public void DataTransferOrderChangeResponse7N_ShouldGetDeserializeCorrectly(string raw, Packet expected)
    {
        var packet = Packet.Deserialize(raw);

        Assert.Equivalent(expected, packet);
    }
}
