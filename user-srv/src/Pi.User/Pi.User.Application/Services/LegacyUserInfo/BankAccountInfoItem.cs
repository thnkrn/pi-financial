namespace Pi.User.Application.Services.LegacyUserInfo
{
    public class BankAccountInfoItem
    {
        public BankAccountInfoItem(
            string tradingAccountNo,
            string accountType,
            string transactionType,
            string rPType,
            string bankCode,
            string bankAccountNo,
            string bankAccountType,
            string payType,
            DateOnly? effectiveDate,
            DateOnly? endDate,
            string accountCodeType,
            string exchangeMarket)
        {
            TradingAccountNo = tradingAccountNo;
            AccountType = accountType;
            TransactionType = transactionType;
            RPType = rPType;
            BankCode = bankCode;
            BankAccountNo = bankAccountNo;
            BankAccountType = bankAccountType;
            PayType = payType;
            EffectiveDate = effectiveDate;
            EndDate = endDate;
            AccountCodeType = accountCodeType;
            ExchangeMarket = exchangeMarket;
        }

        public string TradingAccountNo { get; set; }
        public string AccountType { get; set; }
        public string TransactionType { get; set; }
        public string RPType { get; set; }
        public string BankCode { get; set; }
        public string BankAccountNo { get; set; }
        public string BankAccountType { get; set; }
        public string PayType { get; set; }
        public DateOnly? EffectiveDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string AccountCodeType { get; set; }
        public string ExchangeMarket { get; set; }
    }
}

