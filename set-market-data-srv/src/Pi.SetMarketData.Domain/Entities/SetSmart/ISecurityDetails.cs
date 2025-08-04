namespace Pi.SetMarketData.Domain.Entities.SetSmart;

public class SecurityDetails
{
    public int ISecurity { get; set; }
    public string? NSecurity { get; set; }
    public string? NSecurityE { get; set; }
    public decimal? ZMultiplier { get; set; }
    public decimal? ZExercise { get; set; }
    public double? QFirstRatio { get; set; }
    public double? QLastRatio { get; set; }
    public DateTime? DLastExercise { get; set; }
    public int? QTtm { get; set; }
    public DateTime? DFirstTrade { get; set; }
    public DateTime? DLastTrade { get; set; }
}