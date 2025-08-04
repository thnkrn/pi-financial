namespace Pi.PortfolioService.Models;

public enum Product
{
    Unknown,
    Cash,
    CashBalance,
    CreditBalanceSbl,
    Crypto,
    Derivatives,
    GlobalEquities,
    Funds,
    Bond,
    CashSbl,
    CashBalanceSbl,
    CreditBalance,
    StructureNoteOnShore,
    Dr,
    LiveX,
    BorrowCash,
    BorrowCashBalance
}

public class TradingAccount
{
    public TradingAccount(Guid userId, string customerCode, string tradingAccountNo, Product product)
    {
        CustomerCode = customerCode;
        TradingAccountNo = tradingAccountNo;
        UserId = userId;
        Product = product;
    }

    public string CustomerCode { get; }
    public string TradingAccountNo { get; }
    public Guid UserId { get; }
    public Product Product { get; }
}
