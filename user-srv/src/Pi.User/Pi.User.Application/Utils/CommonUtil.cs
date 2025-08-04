using System.Text.RegularExpressions;

namespace Pi.User.Application.Utils;

public class CommonUtil
{
    public static readonly Dictionary<string, string> SUPPORT_FILE_TYPES = new Dictionary<string, string> {
        { ".pdf", "application/pdf" },
        { ".png", "image/png" },
        { ".jpeg", "image/jpeg" },
        { ".jpg", "image/jpeg" },
        { ".heic", "image/jpeg" },
        { ".heif", "image/jpeg" },
    };
    public static string RemoveSpecialCharacters(string input)
    {
        // Define a HashSet to store the characters to remove
        var charactersToRemove = new HashSet<char> { ',', '\'', '“', '\t', '^', '|', '\\', '"' };

        // Use string.Replace to remove the characters
        foreach (var c in charactersToRemove)
        {
            input = input.Replace(c.ToString(), string.Empty);
        }

        // Use regular expression to remove consecutive spaces
        input = Regex.Replace(input, @"\s+", " ", RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(200));

        // Remove leading and trailing spaces
        input = input.Trim();

        return input;
    }

    public static bool IsSupportFileExtension(string extension)
    {
        return SUPPORT_FILE_TYPES.ContainsKey(extension.ToLower());
    }

    public static string RandomNumberCode()
    {
        Random RandomGenerator = new Random();
        return RandomGenerator.Next(100000, 1000000).ToString();
    }
}