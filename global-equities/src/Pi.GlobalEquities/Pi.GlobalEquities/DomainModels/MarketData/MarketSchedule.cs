using Pi.GlobalEquities.Models.MarketData;

namespace Pi.GlobalEquities.DomainModels.MarketData;

public class MarketSchedule
{
    public Session Session { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public Dictionary<OrderType, IEnumerable<OrderDuration>> OrderDuration { get; set; }
}

