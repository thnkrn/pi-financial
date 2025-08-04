namespace Pi.FundMarketData.Worker.Services.FundMarket.Morningstar
{
    public class MorningstarOptions
    {
        public const string Name = "Morningstar";

        public string ServerUrl { get; init; }
        public int AccessTokenLifetimeInDay { get; init; }
        public string AccountCode { get; init; }
        public string Password { get; init; }
        public string UniverseId { get; init; }
        public string GetHistoricalNavsServiceUrl { get; init; }
        public string GetFundBasicInfosServiceUrl { get; init; }
        public string GetAssetClassAllocationsServiceUrl { get; init; }
        public string GetStockSectorAllocationsServiceUrl { get; init; }
        public string GetFeesAndExpensesServiceUrl { get; init; }
        public string GetFundPerformancesServiceUrl { get; init; }
        public string GetTop25UnderlyingHoldingServiceUrl { get; init; }
        public string GetHistoricalDistributionServiceUrl { get; init; }
        public string GetRegionalAllocationsServiceUrl { get; init; }
    }
}
