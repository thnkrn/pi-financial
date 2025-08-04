namespace Pi.StructureNotes.Domain.Models;

public class AssetSummary
{
    public decimal? TotalCostValue => _cost;
    public decimal? TotalMarketValue => _marketValue;
    public decimal? TotalGain => _gain;

    private decimal? _cost;
    private decimal? _marketValue;
    private decimal? _gain;

    public AssetSummary(params AssetSummary[] summaries)
    {
        foreach (var s in summaries)
        {
            _cost = _cost.HasValue ? _cost + (s._cost ?? 0) : s._cost;
            _marketValue = _marketValue.HasValue ? _marketValue + (s._marketValue ?? 0) : s._marketValue;
            _gain = _gain.HasValue ? _gain + (s._gain ?? 0) : s._gain;
        }
    }

    public AssetSummary(IEnumerable<IAsset> assets)
    {
        foreach (IAsset s in assets)
        {
            _cost = _cost.HasValue ? _cost + (s.CostValue ?? 0) : s.CostValue;
            _marketValue = _marketValue.HasValue ? _marketValue + (s.MarketValue ?? 0) : s.MarketValue;
            _gain = _gain.HasValue ? _gain + (s.Gain ?? 0) : s.Gain;
        }
    }
}
