using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Pi.Common.CommonModels;

namespace Pi.GlobalEquities.DomainModels;

public class Order : IOrder
{
    private string _id;

    [BsonId]
    public required string Id
    {
        get => _id;
        set
        {
            if (string.IsNullOrWhiteSpace(_id) || _id == value)
            {
                _id = value;
            }
            else
            {
                throw new Exception("Can not change order Id");
            }
        }
    }
    public string GroupId { get; init; }

    private string _userId;

    public required string UserId
    {
        get => _userId;
        set
        {
            if (string.IsNullOrWhiteSpace(_userId) || _userId == value)
            {
                _userId = value;
            }
            else
            {
                throw new Exception("Order owner cannot be changed (UserId)");
            }
        }
    }

    private string _accountId;

    public required string AccountId
    {
        get => _accountId;
        set
        {
            if (string.IsNullOrWhiteSpace(_accountId) || _accountId == value)
            {
                _accountId = value;
            }
            else
            {
                throw new Exception("Order owner cannot be changed (AccountId)");
            }
        }
    }

    public required string Venue { get; init; }
    public string SymbolId => $"{Symbol}.{Venue}";
    public required string Symbol { get; init; }
    [BsonRepresentation(BsonType.String)] public required OrderType OrderType { get; init; }
    [BsonRepresentation(BsonType.String)] public required OrderSide Side { get; init; }
    [BsonRepresentation(BsonType.String)] public OrderDuration Duration { get; init; } = OrderDuration.Day;

    [BsonRepresentation(BsonType.String)] public Currency Currency => CurrencyFromVenue(Venue);

    private decimal _quantity;

    public decimal Quantity
    {
        get => _quantity;
        init => _quantity = value;
    }

    public decimal QuantityFilled { get; private set; }

    public decimal QuantityCancelled =>
        Status == OrderStatus.PartiallyMatched || Status == OrderStatus.Cancelled || Status == OrderStatus.Rejected
            ? Quantity - QuantityFilled
            : 0;

    private decimal? _limitPrice;

    public decimal? LimitPrice
    {
        get => _limitPrice;
        init => _limitPrice = value;
    }

    private decimal? _stopPrice;

    public decimal? StopPrice
    {
        get => _stopPrice;
        init => _stopPrice = value;
    }

    public decimal? AverageFillPrice { get; private set; }
    public decimal? TotalFillPrice { get; private set; }

    private IEnumerable<OrderFill> _fills;

    public IEnumerable<OrderFill> Fills
    {
        get => _fills;
        init => UpdateFills(value);
    }

    private OrderStatus _status;

    [BsonRepresentation(BsonType.String)]
    public OrderStatus Status
    {
        get => _status;
        set => _status = value;
    }

    [BsonRepresentation(BsonType.String)]
    public OrderReason? StatusReason { get; set; }

    public ProviderInfo ProviderInfo { get; set; }

    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }

    private bool _hasBeenModified;

    [BsonIgnore]
    public bool HasBeenModified
    {
        get => _hasBeenModified;
        private set
        {
            _hasBeenModified = _hasBeenModified ? _hasBeenModified : value;
            if (_hasBeenModified)
                UpdatedAt = DateTime.UtcNow;
        }
    }
    public Channel Channel { get; init; }
    public OrderTransaction Transaction { get; set; }

    public void SetOwner(string userId, string accountId)
    {
        UserId = userId;
        AccountId = accountId;
    }

    public void SetOwner(IAccount account)
    {
        UserId = account.UserId;
        AccountId = account.Id;
    }

    public bool Update(IOrderUpdates updates, out IEnumerable<PropertyChange> changes)
    {
        IOrderValues values = updates;
        IOrderStatus status = updates;

        var isValueModified = Update(values, out var valueChanges);
        var isStatusModified = Update(status, out var statusChanges);

        changes = valueChanges.Union(statusChanges);
        HasBeenModified = isValueModified || isStatusModified;
        return HasBeenModified;
    }

    public bool Update(IOrderValues values, out IEnumerable<PropertyChange> changes)
    {
        if (values == null)
            throw new ArgumentNullException(nameof(values));

        var changeList = new List<PropertyChange>();

        if (PropertyChange.CheckChange(nameof(LimitPrice), LimitPrice, values.LimitPrice, changeList))
        {
            _limitPrice = values.LimitPrice;
        }

        if (PropertyChange.CheckChange(nameof(StopPrice), StopPrice, values.StopPrice, changeList))
        {
            _stopPrice = values.StopPrice;
        }

        if (PropertyChange.CheckChange(nameof(Quantity), Quantity, values.Quantity, changeList))
        {
            _quantity = values.Quantity;
        }

        changes = changeList;
        HasBeenModified = changes.Any();
        return HasBeenModified;
    }

    public bool Update(IOrderStatus update, out IEnumerable<PropertyChange> changes)
    {
        if (update == null)
            throw new ArgumentNullException(nameof(update));

        var changeList = new List<PropertyChange>();

        if (string.IsNullOrWhiteSpace(_id))
            _id = update.ProviderId;

        Id = update.ProviderInfo.OrderId;

        if (PropertyChange.CheckChange(nameof(ProviderInfo.ModifiedAt), ProviderInfo?.ModifiedAt,
                update.ProviderInfo?.ModifiedAt, changeList))
        {
            ProviderInfo = update.ProviderInfo;
        }

        if (PropertyChange.CheckChange(nameof(Status), Status, update.Status, changeList))
        {
            Status = update.Status;
            ProviderInfo = update.ProviderInfo;
        }

        if (PropertyChange.CheckChange(nameof(Fills), Fills, update.Fills, changeList))
        {
            UpdateFills(update.Fills);
            ProviderInfo = update.ProviderInfo;
        }

        changes = changeList;
        HasBeenModified = changes.Any();
        return HasBeenModified;
    }

    private void UpdateFills(IEnumerable<OrderFill> value)
    {
        _fills = _fills == null
            ? value ?? Enumerable.Empty<OrderFill>()
            : _fills.Union(value);

        QuantityFilled = _fills.Sum(x => x.Quantity);
        TotalFillPrice = _fills.Sum(x => x.TotalPrice);
        if (QuantityFilled != 0)
        {
            AverageFillPrice = TotalFillPrice / QuantityFilled;
        }
    }

    public static Currency CurrencyFromVenue(string venue) => venue == "HKEX" ? Currency.HKD : Currency.USD;

    public bool IsFinalStatus()
    {
        return Status is OrderStatus.Matched or OrderStatus.Cancelled or OrderStatus.PartiallyMatched or OrderStatus.Rejected;
    }
}
