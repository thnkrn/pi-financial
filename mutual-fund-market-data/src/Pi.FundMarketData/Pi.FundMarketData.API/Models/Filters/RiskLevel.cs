using System.ComponentModel;

namespace Pi.FundMarketData.API.Models.Filters;

public enum RiskLevel
{
    [Description("Level 1")] Level1 = 1,
    [Description("Level 2")] Level2,
    [Description("Level 3")] Level3,
    [Description("Level 4")] Level4,
    [Description("Level 5")] Level5,
    [Description("Level 6")] Level6,
    [Description("Level 7")] Level7,
    [Description("Level 8")] Level8,
    [Description("Level 8+")] Level9,
}
