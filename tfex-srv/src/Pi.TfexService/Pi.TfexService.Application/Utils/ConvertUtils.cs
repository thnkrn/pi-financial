namespace Pi.TfexService.Application.Utils;

public static class ConvertUtils
{
    public static string ConvertDecimalWithoutRounding<T>(T number, int decimalPoint) where T : struct, IConvertible
    {
        // Validate that decimalPoint is non-negative
        if (decimalPoint < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(decimalPoint), "Decimal point must be greater than 0.");
        }

        // Convert the input number to a double for processing
        var doubleNumber = Convert.ToDouble(number);

        // Correct the exponentiation using Math.Pow
        var factor = Math.Pow(10, decimalPoint);

        // Truncate the number without rounding
        var truncated = Math.Truncate(doubleNumber * factor) / factor;

        // Format the truncated number with the specified number of decimal places
        return truncated.ToString($"N{decimalPoint}");
    }
}