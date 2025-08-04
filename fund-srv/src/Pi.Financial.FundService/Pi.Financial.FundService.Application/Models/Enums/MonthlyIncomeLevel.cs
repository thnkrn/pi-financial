using System.ComponentModel;

namespace Pi.Financial.FundService.Application.Models.Enums;

public enum MonthlyIncomeLevel
{
    [Description("Unknown")] LevelUnknown,
    [Description("LEVEL1")] Level1, // 0 - 15,000
    [Description("LEVEL2")] Level2, // 15,001 - 30,000
    [Description("LEVEL3")] Level3, // 30,001 - 50,000
    [Description("LEVEL4")] Level4, // 50,001 - 100,000
    [Description("LEVEL6")] Level6, // 500,001 - 1,000,000
    [Description("LEVEL5")] Level5, // 100,001 - 500,000
    [Description("LEVEL7")] Level7, // 1,000,001 - 4,000,000
    [Description("LEVEL8")] Level8, // 4,000,001 - 10,000,000
    [Description("LEVEL9")] Level9, // >10,000,000"
}
