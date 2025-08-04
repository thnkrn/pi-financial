namespace Pi.TfexService.Application.Options;

public class SymbolOptions
{
    public const string Options = "Symbol";
    public Multiplier Multiplier { get; set; } = new();
    public TickSize TickSize { get; set; } = new();
    public decimal LotSize { get; set; } = 0;
}

public class Multiplier
{
    public decimal Set50IndexFutures { get; set; }
    public decimal Set50IndexOptions { get; set; }
    public decimal SingleStockFutures { get; set; }
    public SectorIndexFutures SectorIndexFutures { get; set; } = new();
}

public class TickSize
{
    public decimal Set50IndexFutures { get; set; }
    public decimal Set50IndexOptions { get; set; }
    public decimal SingleStockFutures { get; set; }
    public SectorIndexFutures SectorIndexFutures { get; set; } = new();
    public PreciousMetalFutures PreciousMetalFutures { get; set; } = new();
    public CurrencyFutures CurrencyFutures { get; set; } = new();
    public OtherFutures OtherFutures { get; set; } = new();
}

public class SectorIndexFutures
{
    public decimal Bank { get; set; }
    public decimal Ict { get; set; }
    public decimal Energ { get; set; }
    public decimal Comm { get; set; }
    public decimal Food { get; set; }
}

public class PreciousMetalFutures
{
    public decimal GoldFutures { get; set; }
    public decimal GoldFutures10 { get; set; }
    public decimal GoldOnlineFutures { get; set; }
    public decimal SilverOnlineFutures { get; set; }
    public decimal GoldD { get; set; }
}

public class CurrencyFutures
{
    public decimal UsdFutures { get; set; }
    public decimal EurFutures { get; set; }
    public decimal JpyFutures { get; set; }
    public decimal EurUsdFutures { get; set; }
    public decimal UsdJpyFutures { get; set; }
}

public class OtherFutures
{
    public decimal Rss3Futures { get; set; }
    public decimal Rss3dFutures { get; set; }
    public decimal JrfFutures { get; set; }
}