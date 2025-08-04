namespace Pi.GlobalMarketData.DataProcessingService.Utils;

public static class DataManipulation
{
    public static T? GetMaxValue<T>(this IEnumerable<T> sequence)
        where T : IComparable<T>
    {
        var maxIndex = -1;
        T? maxValue = default; // Immediately overwritten anyway

        var index = 0;
        foreach (var value in sequence)
        {
            if (value.CompareTo(maxValue) > 0 || maxIndex == -1)
            {
                maxIndex = index;
                maxValue = value;
            }

            index++;
        }

        return maxValue;
    }

    public static T? GetMinValue<T>(this IEnumerable<T> sequence)
        where T : IComparable<T>
    {
        var maxIndex = -1;
        T? maxValue = default; // Immediately overwritten anyway

        var index = 0;
        foreach (var value in sequence)
        {
            if (value.CompareTo(maxValue) <= 0 || maxIndex == -1)
            {
                maxIndex = index;
                maxValue = value;
            }

            index++;
        }

        return maxValue;
    }
}