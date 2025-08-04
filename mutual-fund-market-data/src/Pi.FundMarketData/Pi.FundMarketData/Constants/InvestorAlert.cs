namespace Pi.FundMarketData.Constants;

public enum InvestorAlert
{
    Unknown = 0,
    RiskFromConcentratedInvestment,
    ConsolidationFund,
}

public static class InvestorAlertExtension
{
    public static IEnumerable<InvestorAlert> ParseInvestorAlerts(IEnumerable<string> values)
    {
        return values
            .Where(val => !string.IsNullOrWhiteSpace(val))
            .Select(val =>
            {
                val = val.Trim();
                if (int.TryParse(val, out int result) && Enum.IsDefined(typeof(InvestorAlert), result))
                {
                    return (InvestorAlert)result;
                }
                return InvestorAlert.Unknown;
            });
    }
}
