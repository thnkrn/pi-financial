using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Pi.FundMarketData.API.Models.Filters;

public enum Dividend
{
    [Description("Dividend")] Yes,
    [Description("No")] No,
}

// DividendV2 is temporary used for galaxy project, Will be removed once the Dividend's issue is fixed
// Dividend currently has issue when generating client for Typescript.
// Enum member: yes, no, y, n, on, off is a reserved word and will be translated to True, False which is not correct
public enum DividendV2
{
    [Description("Dividend")] Dividend,
    [Description("Not Dividend")] NotDividend,
}
