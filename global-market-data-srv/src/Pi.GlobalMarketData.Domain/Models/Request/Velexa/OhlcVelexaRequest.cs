using System.Text.Json.Serialization;

namespace Pi.GlobalMarketData.Domain.Models.Request.Velexa;

public class OhlcRequestDuration(string value)
{
    public string Value { get; private set; } = value;

    public static OhlcRequestDuration D60
    {
        get { return new OhlcRequestDuration("60"); }
    }
    public static OhlcRequestDuration D300
    {
        get { return new OhlcRequestDuration("300"); }
    }
    public static OhlcRequestDuration D600
    {
        get { return new OhlcRequestDuration("600"); }
    }
    public static OhlcRequestDuration D900
    {
        get { return new OhlcRequestDuration("900"); }
    }
    public static OhlcRequestDuration D1800
    {
        get { return new OhlcRequestDuration("1800"); }
    }
    public static OhlcRequestDuration D3600
    {
        get { return new OhlcRequestDuration("3600"); }
    }
    public static OhlcRequestDuration D14400
    {
        get { return new OhlcRequestDuration("14400"); }
    }
    public static OhlcRequestDuration D21600
    {
        get { return new OhlcRequestDuration("21600"); }
    }
    public static OhlcRequestDuration D86400
    {
        get { return new OhlcRequestDuration("86400"); }
    }

    public override string ToString()
    {
        return Value;
    }
}

public class OhlcRequestType(string value)
{
    public string Value { get; private set; } = value;

    public static OhlcRequestType Quotes
    {
        get { return new OhlcRequestType("quotes"); }
    }
    public static OhlcRequestType Trades
    {
        get { return new OhlcRequestType("trades"); }
    }

    public override string ToString()
    {
        return Value;
    }
}

public class OhlcVelexaRequest
{
    [JsonPropertyName("from")]
    public long From { get; set; }

    [JsonPropertyName("to")]
    public long To { get; set; }

    [JsonPropertyName("size")]
    public string? Size { get; set; } = OhlcRequestDuration.D60.Value;

    [JsonPropertyName("type")]
    public string? Type { get; set; } = OhlcRequestType.Quotes.Value;
}
