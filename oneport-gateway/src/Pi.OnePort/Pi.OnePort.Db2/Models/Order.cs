using System.ComponentModel.DataAnnotations.Schema;

namespace Pi.OnePort.Db2.Models;

public class Order : RootOrder
{
    public string? OrderStatus { get; set; }
    public string? RejectCode { get; set; }
    public string? OrderToken { get; set; }
    public string? ControlKey { get; set; }
    [Column("VALIDTILLDATE")]
    public string? ValidTilDate { get; set; }
    public string? ExpireDate { get; set; }
    public string? MktOrdNo { get; set; }
}
