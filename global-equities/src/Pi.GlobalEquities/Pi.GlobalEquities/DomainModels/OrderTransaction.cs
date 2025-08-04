using Pi.Common.CommonModels;

namespace Pi.GlobalEquities.DomainModels;

public class OrderTransaction
{
    public string OrderId { get; init; }

    public Currency Currency { get; private set; }

    public IEnumerable<TransactionItem> Transactions { get; init; }

    public OrderTransaction(string orderId, IEnumerable<TransactionItem> transactions, Currency currency)
    {
        OrderId = orderId;
        Currency = currency;
        Transactions = BuildTransaction(transactions);
    }

    public decimal GetTradeCost(Currency? currency = null)
    {
        var targetCurrency = currency ?? Currency;

        var cost = 0m;
        foreach (var trn in Transactions)
        {
            if (trn.OperationType != OperationType.Trade)
            {
                continue;
            }

            if (trn.Asset == targetCurrency.ToString())
            {
                cost += trn.Value;
            }
            else if (trn.Children.Any())
            {
                var currencyConversionTrn = TryGetCurrencyConversionTransaction(trn, targetCurrency);

                cost += currencyConversionTrn?.Value ?? 0;
            }
        }

        return cost;
    }

    public decimal GetCommission(Currency? currency = null)
    {
        var targetCurrency = currency ?? Currency;

        var commission = 0m;
        foreach (var trn in Transactions)
        {
            if (trn.OperationType != OperationType.Commission)
            {
                continue;
            }

            if (trn.Asset == targetCurrency.ToString())
            {
                commission += trn.Value;
            }
            else if (trn.Children.Any())
            {
                var currencyConversionTrn = TryGetCurrencyConversionTransaction(trn, targetCurrency);

                commission += currencyConversionTrn?.Value ?? 0;
            }
        }

        return commission;
    }

    private static TransactionItem TryGetCurrencyConversionTransaction(TransactionItem trn, Currency currency)
    {
        if (trn == null || trn.Children == null)
            return null;

        var result = trn.Children
            .FirstOrDefault(x => x.OperationType == OperationType.AutoConversion
                                 && x.Asset == currency.ToString());
        if (result != null)
            return result;

        foreach (var cTrn in trn.Children)
        {
            result = TryGetCurrencyConversionTransaction(cTrn, currency);
            if (result != null)
                return result;
        }

        return null;
    }

    public decimal GetTotalCost(Currency? currency = null)
    {
        return GetTradeCost(currency) + GetCommission(currency);
    }

    private static IEnumerable<TransactionItem> BuildTransaction(IEnumerable<TransactionItem> transactions)
    {
        if (transactions == null || !transactions.Any())
            return new List<TransactionItem>();

        var trns = transactions.ToDictionary(x => x.Id);

        foreach (var trn in trns.Values)
        {
            if (trn.ParentId is null)
            {
                continue;
            }

            if (trns.TryGetValue(trn.ParentId, out TransactionItem parent))
            {
                parent.Children.Add(trn);
            }
        }

        return trns.Values.Where(x => x.ParentId is null);
    }
}
