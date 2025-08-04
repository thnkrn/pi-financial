namespace Pi.SetMarketDataWSS.Domain.Entities;

public class Financial
{
    public int FinancialId { get; set; }
    public int InstrumentId { get; set; }
    public string? FinancialType { get; set; }
    public string? Yy { get; set; }
    public string? Q1 { get; set; }
    public string? Q2 { get; set; }
    public string? Q3 { get; set; }
    public string? Q4 { get; set; }
    public string? StatementType { get; set; }
    public string? Units { get; set; }
    public string? LatestFinancials { get; set; }
    public string? Source { get; set; }

    public virtual Instrument Instrument { get; set; }
}