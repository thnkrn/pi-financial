namespace Pi.FundMarketData.Worker.Services.FundMarket.FundConnext
{
    public class FundConnextOptions
    {
        public const string Name = "FundConnext";

        public string ServerUrl { get; init; }
        public string Username { get; init; }
        public string Password { get; init; }
    }
}
