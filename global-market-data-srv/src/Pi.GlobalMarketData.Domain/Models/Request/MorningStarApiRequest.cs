namespace Pi.GlobalMarketData.Domain.Models.Request;

public class StockStatus
{
    private StockStatus(string value) => Value = value;

    public string Value { get; private set; }

    public static StockStatus All => new("All");
    public static StockStatus Active => new("Active");
    public static StockStatus Delisted => new("Delisted");

    public override string ToString() => Value;
}

public class Period
{
    private Period(string value) => Value = value;

    public string Value { get; private set; }
    public static Period Snapshot => new("Snapshot");
    public static Period YearEnd5Year => new("Year_End_5Year");
    public static Period YearEnd10Year => new("Year_End_10Year");
    public static Period YearEnd10FiscalYear => new("Year_End_10FiscalYear");
    public static Period YearEnd1YTD => new("Year_End_1YTD");

    public override string ToString() => Value;
}

public class IdentifierType
{
    private IdentifierType(string value) => Value = value;

    public string Value { get; private set; }
    public static IdentifierType Symbol => new("Symbol");
    public static IdentifierType ExchangeId => new("ExchangeId");
    public static IdentifierType Isin => new("ISIN");
    public static IdentifierType All => new("ALL");

    public override string ToString() => Value;
}

public class DataType
{
    private DataType(string value) => Value = value;

    public string Value { get; private set; }
    public static DataType AOR => new("AOR");
    public static DataType Restated => new("Restated");
    public static DataType Preliminary => new("Preliminary");

    public override string ToString() => Value;
}

public class MorningStarExchangeIdRequest : MorningStarCommonRequest
{
    public string? ExchangeId { get; set; }
}

public class MorningStarStatementTypeRequest : MorningStarCommonRequest
{
    public string? ExchangeId { get; set; }
    public string? StatementType { get; set; }
    public string? DataType { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
}

public class MorningStarStockRequest : MorningStarCommonRequest
{
    public string? ExchangeId { get; set; }
    public StockStatus StockStatus { get; set; } = StockStatus.Active;
}

public class MorningStarPeriodRequest : MorningStarCommonRequest
{
    public string? ExchangeId { get; set; }
    public Period Period { get; set; } = Period.Snapshot;
}

public class MorningStarExcludingPeriodRequest : MorningStarCommonRequest
{
    public string? ExchangeId { get; set; }
    public string? ExcludingFrom { get; set; }
    public string? ExcludingTo { get; set; }
}

public class MorningStarCommonRequest
{
    public IdentifierType IdentifierType { get; set; } = IdentifierType.Symbol;
    public string Identifier { get; set; } = string.Empty;
    public string ResponseType { get; set; } = "Json";
}
