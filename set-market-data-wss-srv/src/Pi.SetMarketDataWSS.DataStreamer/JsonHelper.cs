using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Pi.SetMarketDataWSS.DataStreamer;

public static class JsonHelper
{
    private static readonly Regex WhitespaceRegex = new(@"(""(?:[^""\\]|\\.)*"")|[\s\n\r\t]+", RegexOptions.Compiled);
    private static readonly Regex ColonSpaceRegex = new(@"(?<=([^""]\S)):\s+", RegexOptions.Compiled);
    private static readonly Regex CommaSpaceRegex = new(@"\s*,\s*", RegexOptions.Compiled);
    private static readonly Regex TrailingSpaceRegex = new(@"(""Value"":""[^""]*?)\s+("")", RegexOptions.Compiled);

    private static readonly Regex NewlineInValueRegex =
        new(@"(""Value"":""[^""]*?)[\n\r\t]+("")", RegexOptions.Compiled);

    private static readonly Regex ExtraSpaceInQuotesRegex =
        new(@"(?<=:\s*""[^""]*)\s+(?=[^""]*"")", RegexOptions.Compiled);

    private static readonly Regex JsonCleanerRegex =
        new(@"\s+|[\r\n\t]", RegexOptions.Compiled);

    public static string SimpleCleanJsonMessage(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        var cleanedJson = JsonCleanerRegex.Replace(input,
            match => { return match.Value.Any(c => c is '\r' or '\n' or '\t') ? string.Empty : " "; });

        // Parse the cleaned JSON
        var token = JToken.Parse(cleanedJson);

        // Trim string values
        TrimStringValues(token);

        // Convert back to string
        return token.ToString(Formatting.None);
    }

    private static void TrimStringValues(JToken token)
    {
        switch (token.Type)
        {
            case JTokenType.Object:
                foreach (var prop in token.Children<JProperty>()) TrimStringValues(prop.Value);
                break;
            case JTokenType.Array:
                foreach (var item in token.Children()) TrimStringValues(item);
                break;
            case JTokenType.String:
                ((JValue)token).Value = token.ToString().Trim();
                break;
        }
    }

    public static string CleanJsonMessage(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        try
        {
            var cleanedJson = new StringBuilder(input.Length);

            // Remove \n, \r, \t characters and whitespace outside of quotes
            cleanedJson.Append(WhitespaceRegex.Replace(input, m => m.Groups[1].Success ? m.Value : ""));

            // Remove spaces around colons and commas
            cleanedJson = new StringBuilder(ColonSpaceRegex.Replace(cleanedJson.ToString(), ":"));
            cleanedJson = new StringBuilder(CommaSpaceRegex.Replace(cleanedJson.ToString(), ", "));

            // Trim trailing spaces in "Value" fields
            cleanedJson = new StringBuilder(TrailingSpaceRegex.Replace(cleanedJson.ToString(), "$1$2"));

            // Remove any remaining \n, \r, \t inside "Value" fields
            cleanedJson = new StringBuilder(NewlineInValueRegex.Replace(cleanedJson.ToString(), "$1$2"));

            // Remove extra spaces within quotes
            cleanedJson = new StringBuilder(ExtraSpaceInQuotesRegex.Replace(cleanedJson.ToString(), " "));

            return cleanedJson.ToString();
        }
        catch (RegexMatchTimeoutException)
        {
            // Log the exception and return the original input or handle as appropriate
            return input;
        }
        catch (Exception)
        {
            // Log the exception and return the original input or handle as appropriate
            return input;
        }
    }

    public static bool IsValidJsonMessage(this string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;

        input = input.Trim();
        if ((input.StartsWith("{") && input.EndsWith("}")) ||
            (input.StartsWith("[") && input.EndsWith("]")))
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
}