using Pi.OnePort.TCP.Enums.DataTransfer;
using Pi.OnePort.TCP.Models;
using Pi.OnePort.TCP.Models.Packets;
using Pi.OnePort.TCP.Models.Packets.DataTransfer;

namespace Pi.OnePort.TCP.Tests.PacketDeserialization;

public class DataTransferOrderChangeResponse7NTests
{
    public static IEnumerable<object[]> GetTestData()
    {
        return new List<object[]>
        {
            new object[]
            {
                "00098262DT20231110-1230067n000000000000000000000000000000000000000000000000000000000000000000000343849004       01669771  2               0               8803The price may not be improved in this session state                                                                             ",
                new Packet(
                    new PacketDataTransfer(
                        new DataTransferOrderChangeResponse7N()
                        {
                            RefOrderId = "0000000000000000000000000000000000000000000000000000000000000000",
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
                )
                {
                    Sequence = 98262,
                    Timestamp = DateTime.Parse("2023-11-10T12:30:06.0000000")
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


