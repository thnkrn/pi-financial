namespace Pi.SetMarketDataWSS.Domain.Entities;

public class CorporateAction
{
    public int CorporateActionId { get; set; }
    public int InstrumentId { get; set; }
    public string? Type { get; set; }
    public string? Date { get; set; }
    public string? Code { get; set; }

    public virtual Instrument Instrument { get; set; }
}