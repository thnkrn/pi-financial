namespace Pi.SetService.API.Extensions;

public static class DecimalExtensions
{
    public static decimal Round(this decimal value, int digit = 4)
    {
        return decimal.Round(value, digit);
    }

    public static decimal Round(this decimal? value, int digit = 4)
    {
        return decimal.Round(value ?? 0, digit);
    }
}
