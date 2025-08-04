namespace Pi.User.Application.Services.LegacyUserInfo
{
    public class BankAccountInfo
    {
        public BankAccountInfo(string customerCode, IEnumerable<BankAccountInfoItem> items, int totalItems)
        {
            CustomerCode = customerCode;
            Items = items;
            TotalItems = totalItems;
        }

        public string CustomerCode { get; }
        public IEnumerable<BankAccountInfoItem> Items { get; set; }
        public int TotalItems { get; set; }
    }
}

