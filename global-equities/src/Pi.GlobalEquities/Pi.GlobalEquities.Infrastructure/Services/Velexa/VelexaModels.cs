using Pi.Common.ObjectModel;

namespace Pi.GlobalEquities.Infrastructure.Services.Velexa;

public class VelexaModel
{
    public class OrderRequest
    {
        public required string accountId { get; init; }
        public required string symbolId { get; init; }
        public required LowerString side { get; init; }
        public required string quantity { get; init; }
        public required LowerString orderType { get; init; }
        public string? stopPrice { get; init; }
        public string? limitPrice { get; init; }
        public required LowerString duration { get; init; }
        public string? clientTag { get; init; }
        public string? ocoGroup { get; init; }
    }

    public class ModifyRequest
    {
        public required string action { get; set; }
        public Parameter? parameters { get; set; }
    }

    public class Parameter
    {
        public string? quantity { get; set; }
        public string? limitPrice { get; set; }
        public string? stopPrice { get; set; }
        public string? priceDistance { get; set; }
    }

    public class OrderResponse
    {
        public required string accountId { get; set; }
        public string? clientTag { get; set; }
        public string currentModificationId { get; set; }
        public string orderId { get; set; }
        public OrderParameters orderParameters { get; set; }
        public OrderState orderState { get; set; }
        public DateTime placeTime { get; set; }
        public string username { get; set; }
    }

    public class Fill
    {
        public string quantity { get; set; }
        public string price { get; set; }
        public int position { get; set; }
        public DateTime timestamp { get; set; }
    }

    public class OrderParameters
    {
        public string symbolId { get; set; }
        public string side { get; set; }
        public string quantity { get; set; }
        public string limitPrice { get; set; }
        public string stopPrice { get; set; } // This property is only present in some objects
        public string orderType { get; set; }
        public string duration { get; set; }
        public string ocoGroup { get; set; }
        public string ifDoneParentId { get; set; }

        public string Symbol => symbolId.Split('.')[0];
        public string Venue => symbolId.Split('.')[1];
    }

    public class OrderState
    {
        public string status { get; set; }
        public DateTime lastUpdate { get; set; }
        public IEnumerable<Fill> fills { get; set; }
        public string reason { get; set; }
    }

    public class PositionResponse
    {
        public long timestamp { get; set; }
        public string freeMoney { get; set; }
        public string netAssetValue { get; set; }
        public string marginUtilization { get; set; }
        public object sessionDate { get; set; }
        public string currency { get; set; }
        public string accountId { get; set; }
        public string moneyUsedForMargin { get; set; }

        public AvailableCurrency[] currencies { get; set; }
        public VelexaPosition[] positions { get; set; }
    }

    public class AvailableCurrency
    {
        public string code { get; set; }
        public string convertedValue { get; set; }
        public string value { get; set; }
    }

    public class VelexaPosition
    {
        public string? convertedPnl { get; set; }
        public string? symbolId { get; set; }
        public string? convertedValue { get; set; }
        public string? price { get; set; }
        public string? symbolType { get; set; }
        public string? value { get; set; }
        public string? quantity { get; set; }
        public string? pnl { get; set; }
        public string? currency { get; set; }
        public string? averagePrice { get; set; }
    }

    public class TransactionResponse
    {
        public string? symbolId { get; set; }
        public long timestamp { get; set; }
        public string? uuid { get; set; }
        public int? orderPos { get; set; }
        public string? accountId { get; set; }
        public string? valueDate { get; set; }
        public long id { get; set; }
        public string? sum { get; set; }
        public string? orderId { get; set; }
        public string? operationType { get; set; }
        public string? parentUuid { get; set; }
        public string? asset { get; set; }
    }

    public class ExchangeRate
    {
        public string? pair { get; set; }
        public string? rate { get; set; }
        public string? symbolId { get; set; }
    }

    public class ScheduleResponse
    {
        public IEnumerable<IntervalResponse>? intervals { get; set; }
    }

    public class IntervalResponse
    {
        public string? name { get; set; }
        public PeriodResponse? period { get; set; }
        public OrderTypeResponse? orderTypes { get; set; }
    }

    public class PeriodResponse
    {
        public long start { get; set; }
        public long end { get; set; }
    }

    public class OrderTypeResponse
    {
        public IEnumerable<string>? limit { get; set; }
        public IEnumerable<string>? market { get; set; }
        public IEnumerable<string>? stop { get; set; }
        public IEnumerable<string>? stop_limit { get; set; }
        public IEnumerable<string>? twap { get; set; }
        public IEnumerable<string>? trailing_stop { get; set; }
        public IEnumerable<string>? iceberg { get; set; }
        public IEnumerable<string>? pov { get; set; }
        public IEnumerable<string>? vwap { get; set; }
    }
}
