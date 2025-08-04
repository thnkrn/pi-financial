using Pi.Common.CommonModels;

namespace Pi.GlobalEquities.API.Models.Responses;

public class AccountSummaryResponse
{
    /// <summary>
    /// CustCode-2
    /// </summary>
    public string TradingAccountNo { get; init; }
    public string TradingAccountNoDisplay { get; init; }
    public decimal UpnlPercentage { get; init; }
    public ExchangeRate ExchangeRate { get; init; }
    public IEnumerable<AccountSummaryValueResponse> Values { get; init; }
    public IEnumerable<PositionResponse> Positions { get; init; }
    public class AccountSummaryValueResponse
    {
        public Currency Currency { get; init; }
        public decimal NetAssetValue { get; init; }
        public decimal MarketValue { get; init; }
        public decimal Cost { get; init; }
        public decimal Upnl { get; init; }
        public decimal UnusedCash { get; init; }
        public decimal AccountLimit { get; init; }
        public decimal LineAvailable { get; init; }
    }

    public class PositionResponse
    {
        public string Symbol { get; init; }
        public string Venue { get; init; }
        public Currency Currency { get; init; }
        public decimal EntireQuantity { get; init; }
        public decimal AvailableQuantity { get; init; }
        public decimal LastPrice { get; init; }
        public decimal MarketValue { get; init; }
        public decimal AveragePrice { get; init; }
        public decimal Upnl { get; init; }
        public decimal Cost { get; init; }
        public decimal UpnlPercentage { get; init; }
        public string Logo { get; init; }
    }
}



