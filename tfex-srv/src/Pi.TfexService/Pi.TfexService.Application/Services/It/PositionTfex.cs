namespace Pi.TfexService.Application.Services.It;

public record PositionTfexResponseData(
    int Code,
    string Message,
    int Count,
    DateTime Timestamp,
    PositionTfex Data
);

public record PositionTfex(
    string SendDate,
    string SendTime,
    string OutstdDate,
    string CustCode,
    string CustAcct,
    string Account,
    string XchgMkt,
    decimal? Equity,
    decimal? ExcessEquity,
    decimal? MktVal,
    decimal? UnrealizePl,
    decimal? RealizePl,
    List<PositionItem>? PositionList
);

public record PositionItem(
    string ShareCode,
    int LongUnit,
    int ShortUnit,
    decimal AccMtm,
    decimal AvgCostUsdLong,
    decimal AvgCostUsdShort,
    decimal AvgCostThbLong,
    decimal AvgCostThbShort,
    string FxCode,
    decimal SettlementPrice,
    decimal Multiplier
);
