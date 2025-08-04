using System.Collections;

namespace Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;

public class EquityMarginInfo(string symbol, decimal rate, bool isTurnoverList)
{
    public string Symbol { get; set; } = symbol;
    public decimal Rate { get; set; } = rate;
    public bool IsTurnoverList { get; set; } = isTurnoverList;
}