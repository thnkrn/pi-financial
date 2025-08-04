using System.Text.RegularExpressions;

namespace Pi.OnePort.TCP.Utils;

public static partial class RegexUtils
{
    [GeneratedRegex("[^a-zA-Z0-9&-]")]
    private static partial Regex SecSymbolRegex();

    public static string SanitizeSecSymbol(string secSymbol)
        => SecSymbolRegex().Replace(secSymbol, "");
}
