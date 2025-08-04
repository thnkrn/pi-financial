namespace Pi.GlobalEquities.Models;

public class StaticUrl
{
    private static bool _isInitialized = false;

    public static string PiLogoUrl { get; private set; }

    public static void Init(
        string piLogoUrl)
    {
        if (_isInitialized)
            return;

        PiLogoUrl = Path.Combine(piLogoUrl, "GE_PRODUCT_LOGO");

        _isInitialized = true;
    }
}
