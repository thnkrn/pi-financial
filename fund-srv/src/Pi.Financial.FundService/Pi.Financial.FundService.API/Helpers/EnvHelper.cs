namespace Pi.Financial.FundService.API.Helpers;

public static class EnvHelper
{
    public static bool IsProduction()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        return env.ToLower() == "production";
    }
}
