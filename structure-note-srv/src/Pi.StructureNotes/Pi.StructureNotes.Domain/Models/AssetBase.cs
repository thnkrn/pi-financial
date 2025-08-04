namespace Pi.StructureNotes.Domain.Models;

public abstract class AssetBase : IAsset
{
    private decimal? _costValue;

    private string _currency;

    private decimal? _marketValue;

    public required string Symbol { get; init; }

    private DateTimeOffset? _asOff;
    public required DateTimeOffset? AsOf
    {
        get => _asOff;
        init => _asOff = value?.ToLocalTime();
    }
    public required string Id { get; init; }
    public required string AccountId { get; init; }

    public required string Currency
    {
        get => _currency;
        init => _currency = value?.ToUpper();
    }

    public virtual required decimal? CostValue
    {
        get => _costValue;
        init => _costValue = value;
    }

    public virtual required decimal? MarketValue
    {
        get => _marketValue;
        init => _marketValue = value;
    }

    public virtual decimal? Gain
    {
        get
        {
            if (!MarketValue.HasValue || !CostValue.HasValue)
            {
                return null;
            }

            return MarketValue - CostValue;
        }
    }

    public virtual decimal? PercentChange
    {
        get
        {
            if (!Gain.HasValue)
            {
                return null;
            }

            if (CostValue == 0)
            {
                return 100;
            }

            return Gain.Value * 100 / CostValue;
        }
    }

    public string Logo { get; protected set; }

    public void SetCurrency(string currency, IExchangeRateLookup lookup)
    {
        currency = currency.ToUpper();
        if (_currency == currency)
        {
            return;
        }

        lookup.TryGetExchangeRate(_currency, currency, out decimal? rate);
        SetCurrency(currency, rate);
    }

    protected virtual void SetCurrency(string currency, decimal? rate)
    {
        _currency = currency?.ToUpper();
        _costValue = _costValue * rate;
        _marketValue = _marketValue * rate;
    }
}
