namespace Pi.StructureNotes.Domain.Models;

public class Stock : AssetBase
{
    private decimal? _costPrice;

    private decimal? _marketPrice;
    public required int? Units { get; init; }
    public required int? Available { get; init; }

    public required decimal? CostPrice
    {
        get => _costPrice;
        init => _costPrice = value;
    }

    public required decimal? MarketPrice
    {
        get => _marketPrice;
        init => _marketPrice = value;
    }

    public override required decimal? CostValue
    {
        get => _costPrice * Units;
        init
        {
            if (value != null)
            {
                throw new Exception($"{nameof(Stock)}.{nameof(CostValue)} is derived from " +
                                    $"other properties and must be initialized with null");
            }
        }
    }

    public override required decimal? MarketValue
    {
        get => _marketPrice * Units;
        init
        {
            if (value != null)
            {
                throw new Exception($"{nameof(Stock)}.{nameof(MarketValue)} is derived from " +
                                    $"other properties and must be initialized with null");
            }
        }
    }

    public decimal? UnitGain => _marketPrice - _costPrice;

    protected override void SetCurrency(string currency, decimal? rate)
    {
        base.SetCurrency(currency, rate);
        _costPrice = _costPrice * rate;
        _marketPrice = _marketPrice * rate;
    }

    public void SetStockPrice(StockPrice stockPrice)
    {
        if (stockPrice == null)
        {
            throw new ArgumentNullException(nameof(stockPrice));
        }

        if (stockPrice.Currency != Currency)
        {
            throw new ArgumentException(
                $"Call {nameof(Stock)}.{nameof(SetCurrency)} before setting price to different currency");
        }

        _marketPrice = stockPrice.Value;
    }

    public bool TrySetLogo(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException(nameof(url));
        }

        if (url.EndsWith("/"))
        {
            url = url.Substring(0, url.Length - 1);
        }

        try
        {
            string[] symbolArr = Symbol.Split('.');
            if (symbolArr.Length == 2)
            {
                string market = symbolArr[0];
                string symbol = symbolArr[1];
                string logo = $"{url}/{market}_{symbol}.svg";
                Logo = logo;
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }
}
