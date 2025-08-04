namespace Pi.FundMarketData.Constants
{
    public static class StaticUrl
    {
        private static bool _isInitialized = false;
        public static string FactSheetServerUrl { get; private set; }
        public static string AmcLogoCdnServerUrl { get; private set; }

        public static void Init(
            string factSheetServerUrl,
            string amcLogoCdnServerUrl)
        {
            if (_isInitialized)
                return;

            FactSheetServerUrl = factSheetServerUrl;
            AmcLogoCdnServerUrl = amcLogoCdnServerUrl;

            _isInitialized = true;
        }
    }
}
