namespace Pi.OnePort.Db2.Models;

public class OfflineOrder : RootOrder
{
    public required string OrderStatus { get; set; }
    public required string RejectCode { get; set; }
    public required string DelFlag { get; set; }
}
