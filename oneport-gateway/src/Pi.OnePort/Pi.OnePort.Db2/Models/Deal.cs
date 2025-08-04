namespace Pi.OnePort.Db2.Models;

public abstract class Deal
{
    public required ulong OrderNo { get; set; }
    public required int ConfirmNo { get; set; }
    public decimal? DealVolume { get; set; }
    public decimal? DealPrice { get; set; }
    public string? DealTime { get; set; }
    public string? DealDate { get; set; }
    public decimal? SumComm { get; set; }
    public decimal? SumVat { get; set; }
}
