using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Pi.SetMarketData.Application.Helper;

public static partial class JsonHelper
{
    private static readonly Regex JsonCleanerRegex = CleanerRegex();

    public static string SimpleCleanJsonMessage(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        try
        {
            // Parse the input JSON
            var jsonToken = JToken.Parse(input);

            // Clean all string values in the JSON structure
            CleanJsonStringValues(jsonToken);

            // Convert back to string
            return jsonToken.ToString(Formatting.None);
        }
        catch (JsonReaderException)
        {
            // If parsing fails, attempt to clean the input as a raw string
            return CleanRawString(input);
        }
    }

    private static void CleanJsonStringValues(JToken token)
    {
        switch (token.Type)
        {
            case JTokenType.Object:
                foreach (var prop in token.Children<JProperty>())
                    CleanJsonStringValues(prop.Value);
                break;
            case JTokenType.Array:
                foreach (var item in token.Children())
                    CleanJsonStringValues(item);
                break;
            case JTokenType.String:
                var cleanedValue = CleanRawString(token.Value<string>());
                ((JValue)token).Value = cleanedValue;
                break;
        }
    }

    private static string CleanRawString(string? input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        var cleanedString = JsonCleanerRegex.Replace(input,
            match => match.Value.Any(c => c is '\r' or '\n' or '\t') ? string.Empty : " ");

        return cleanedString.Trim();
    }

    public static bool IsValidJsonMessage(this string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;

        input = input.Trim();
        if ((input.StartsWith('{') && input.EndsWith('}')) ||
            (input.StartsWith('[') && input.EndsWith(']')))
            try
            {
                JToken.Parse(input);
                return true;
            }
            catch (JsonReaderException)
            {
                return false;
            }

        return false;
    }

    [GeneratedRegex(@"\s+|[\r\n\t]", RegexOptions.Compiled)]
    private static partial Regex CleanerRegex();
}