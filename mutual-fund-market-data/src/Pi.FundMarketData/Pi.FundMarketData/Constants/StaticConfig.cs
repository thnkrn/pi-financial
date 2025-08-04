namespace Pi.FundMarketData.Constants
{
    public static class StaticConfig
    {
        public static TimeSpan CutOffTimeDeduction { get; private set; }

        public static void Init(
            TimeSpan cutOffTimeDeduction)
        {
            CutOffTimeDeduction = cutOffTimeDeduction;
        }
    }
}
