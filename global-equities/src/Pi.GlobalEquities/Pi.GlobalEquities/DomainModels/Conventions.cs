namespace Pi.GlobalEquities.DomainModels;

public static class Conventions
{
    public static class OrderPrefix
    {
        private const string PREFIX = "GE01|";
        public static bool IsGeId(string str) => !string.IsNullOrWhiteSpace(str) && str.StartsWith(PREFIX);
        public static string Create(string userId, string accountId) => $"{PREFIX}{userId}|{accountId}";

        public static OrderTagInfo Extract(string str)
        {
            var splitData = str.Split("|");
            if (splitData.Length < 3 || splitData.Length > 4)
                throw new InvalidDataException($"{str} is not valid.");

            OrderType? orderType = null;
            if (splitData.Length == 4)
            {
                var text = splitData[3];
                var index = text.IndexOf('=');
                var orderTypeText = (index >= 0 && index < text.Length - 1) ? text[(index + 1)..] : string.Empty;

                if (Enum.TryParse(orderTypeText, out OrderType type))
                    orderType = type;
            }

            return new OrderTagInfo
            {
                UserId = splitData[1],
                AccountId = splitData[2],
                OrderType = orderType
            };
        }

    }
}
