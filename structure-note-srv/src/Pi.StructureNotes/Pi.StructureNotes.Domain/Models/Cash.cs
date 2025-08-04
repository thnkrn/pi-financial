using System.Text.Json.Serialization;

namespace Pi.StructureNotes.Domain.Models;

public class Cash : AssetBase
{
    private decimal? _gain;

    [JsonIgnore]
    public decimal? CashGain
    {
        get => _gain;
        init => _gain = value;
    }

    public override decimal? Gain => _gain;

    protected override void SetCurrency(string currency, decimal? rate)
    {
        base.SetCurrency(currency, rate);
        _gain = _gain * rate;
    }
}
