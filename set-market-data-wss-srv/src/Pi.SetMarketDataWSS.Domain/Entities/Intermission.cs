namespace Pi.SetMarketDataWSS.Domain.Entities;

public class Intermission
{
    public int IntermissionId { get; set; }
    public int InstrumentId { get; set; }
    public int From { get; set; }
    public int To { get; set; }

    public virtual Instrument Instrument { get; set; }
}