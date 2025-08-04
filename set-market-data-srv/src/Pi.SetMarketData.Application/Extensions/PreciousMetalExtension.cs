using Pi.SetMarketData.Application.Constants;

namespace Pi.SetMarketData.Application.Extensions;

public static class PreciousMetalExtension 
{
    private static readonly Dictionary<PreciousMetalType, string> Description = new()
        {
            { PreciousMetalType.GF10, "Gold 10 Baht" },
            { PreciousMetalType.GF, "Gold 50 Baht" },
            { PreciousMetalType.GO, "Gold Online" },
            { PreciousMetalType.SVF, "Silver Online" }
        };

    public static bool TryGetDescription(this string input, out string? description)
    {
        // Attempt to parse the string into a GoldType
        if ((Enum.TryParse(input, ignoreCase: true, out PreciousMetalType goldType)) && (Description.TryGetValue(goldType, out description)))
        {
                return true;
        }

        // Default case: if parsing or lookup fails
        description = null;
        return false;
    }
}