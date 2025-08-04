namespace Pi.FundMarketData.Constants
{
    public static class AssetClassFocus
    {
        public const string FixedIncome = "Fixed Income";
        public const string Equity = "Equity";
        public const string Mixed = "Mixed";
        public const string Other = "Other";

        public static string GetAssetClassFocus(string firstLetter)
        {
            return firstLetter switch
            {
                "F" => FixedIncome,
                "E" => Equity,
                "M" => Mixed,
                "O" => Other,
                _ => throw new ArgumentOutOfRangeException(nameof(firstLetter), $"Not expected AssetClassFocus value: {firstLetter}.")
            };
        }
    }
}
