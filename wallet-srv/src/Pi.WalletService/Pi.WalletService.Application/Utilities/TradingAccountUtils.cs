using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Utilities;

public static class TradingAccountUtils
{
    public static string GetCustCodeFromTradingAccountNo(string tradingAccountNo)
    {
        try
        {
            return tradingAccountNo[..7];
        }
        catch
        {
            throw new InvalidDataException($"Invalid trading account: {tradingAccountNo}");
        }
    }

    public static IList<string> FindTradingAccountsByProduct(IEnumerable<string> tradingAccountNoList, Product product)
    {
        var lastDigit = product switch
        {
            Product.CashBalance => "8",
            Product.CreditBalance or Product.CreditBalanceSbl => "6",
            Product.Cash => "1",
            Product.GlobalEquities => "2",
            Product.Derivatives => "0",
            Product.Funds => "m",
            Product.Crypto or _ => throw new InvalidDataException($"Invalid product: {product}")
        };

        var result = tradingAccountNoList.Where(x => x.EndsWith(lastDigit)).ToList();

        return result;
    }

    public static string? FindTradingAccountByCustCodeAndProduct(string custCode, List<string> tradingAccountNoList, Product product)
    {
        var lastDigit = product switch
        {
            Product.CashBalance => "8",
            Product.CreditBalance or Product.CreditBalanceSbl => "6",
            Product.Cash => "1",
            Product.GlobalEquities => "2",
            Product.Derivatives => "0",
            Product.Funds => "m",
            Product.Crypto or _ => throw new InvalidDataException($"Invalid product: {product}")
        };

        var result = tradingAccountNoList.Find(x => x.StartsWith(custCode) && x.EndsWith(lastDigit));

        return result;
    }

    public static Product FindProductFromTradingAccount(string tradingAccountNo)
    {
        return TryFindProductFromTradingAccount(tradingAccountNo) ?? throw new InvalidDataException($"Invalid tradingAccounts: {tradingAccountNo}");
    }

    public static Product? TryFindProductFromTradingAccount(string tradingAccountNo)
    {
        var product = tradingAccountNo.Last() switch
        {
            '8' => Product.CashBalance,
            '6' => Product.CreditBalanceSbl,
            '1' => Product.Cash,
            '2' => Product.GlobalEquities,
            '0' => Product.Derivatives,
            'm' => Product.Funds,
            _ => (Product?)null
        };

        return product;
    }
}