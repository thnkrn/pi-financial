namespace Pi.SetMarketDataWSS.Application.Helpers;

public static class VenueHelper
{
    private static readonly HashSet<string> _equityType = ["CS", "CSF", "PS", "PSF", "W", "TSR", "DWC", "DWP", "DR", "ETF", "UT", "UL"];
    private static readonly HashSet<string> _derivativeType = ["FC", "FP", "OEC", "OEP", "WEC", "WEP", "CMB", "SPT"];
    public static string GetVenue(string SecurityType) => SecurityType switch
    {
        "IDX" => "Equity",
        var securityType when _equityType.Contains(securityType) => "Equity",
        var securityType when _derivativeType.Contains(securityType) => "Derivative",
        _ => ""
    };
}
