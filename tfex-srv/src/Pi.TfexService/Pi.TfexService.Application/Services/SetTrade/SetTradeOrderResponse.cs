using Pi.TfexService.Application.Models;

namespace Pi.TfexService.Application.Services.SetTrade;

public sealed record PaginatedSetTradeOrder(
    List<SetTradeOrder> Orders,
    bool HasNextPage
);

public sealed record SetTradeOrder(
    long OrderNo,
    string? TfxOrderNo = null,
    string? AccountNo = null,
    string? EntryId = null,
    DateTime? EntryTime = null,
    DateOnly? TradeDate = null,
    DateTime? TransactionTime = null,
    string? CancelId = null,
    DateTime? CancelTime = null,
    string? Symbol = null,
    Side? Side = null,
    Position? Position = null,
    PriceType? PriceType = null,
    decimal Price = 0M,
    decimal MatchedPrice = 0M,
    int Qty = 0,
    int? IcebergVol = 0,
    int? BalanceQty = 0,
    int MatchQty = 0,
    int CancelQty = 0,
    Validity? Validity = null,
    string? ValidToDate = null,
    string? IsStopOrderNotActivate = null,
    TriggerCondition? TriggerCondition = null,
    string? TriggerSymbol = null,
    decimal? TriggerPrice = 0M,
    TriggerSession? TriggerSession = null,
    string? Status = null,
    string? ShowStatus = null,
    string? StatusMeaning = null,
    int? RejectCode = 0,
    string? RejectReason = null,
    string? Cpm = null,
    string? TrType = null,
    string? TerminalType = null,
    long? VarVersion = 0,
    bool CanCancel = false,
    bool CanChange = false,
    int PriceDigit = 0,
    DateTime? OrderTime = null,
    string? Logo = null,
    string? InstrumentCategory = null,
    decimal? TickSize = null,
    decimal? LotSize = null,
    decimal? Multiplier = null,
    MultiplierType? MultiplierType = null,
    MultiplierUnit? MultiplierUnit = null)
{
    public long OrderNo { get; set; } = OrderNo;
    public string? TfxOrderNo { get; set; } = TfxOrderNo;
    public string? AccountNo { get; set; } = AccountNo;
    public string? EntryId { get; set; } = EntryId;
    public DateTime? EntryTime { get; set; } = EntryTime;
    public DateOnly? TradeDate { get; set; } = TradeDate;
    public DateTime? TransactionTime { get; set; } = TransactionTime;
    public string? CancelId { get; set; } = CancelId;
    public DateTime? CancelTime { get; set; } = CancelTime;
    public string? Symbol { get; set; } = Symbol;
    public Side? Side { get; set; } = Side;
    public Position? Position { get; set; } = Position;
    public PriceType? PriceType { get; set; } = PriceType;
    public decimal Price { get; set; } = Price;
    public decimal MatchedPrice { get; set; } = MatchedPrice;
    public int Qty { get; set; } = Qty;
    public int? IcebergVol { get; set; } = IcebergVol;
    public int? BalanceQty { get; set; } = BalanceQty;
    public int MatchQty { get; set; } = MatchQty;
    public int CancelQty { get; set; } = CancelQty;
    public Validity? Validity { get; set; } = Validity;
    public string? ValidToDate { get; set; } = ValidToDate;
    public string? IsStopOrderNotActivate { get; set; } = IsStopOrderNotActivate;
    public TriggerCondition? TriggerCondition { get; set; } = TriggerCondition;
    public string? TriggerSymbol { get; set; } = TriggerSymbol;
    public decimal? TriggerPrice { get; set; } = TriggerPrice;
    public TriggerSession? TriggerSession { get; set; } = TriggerSession;
    public string? Status { get; set; } = Status;
    public string? ShowStatus { get; set; } = ShowStatus;
    public string? StatusMeaning { get; set; } = StatusMeaning;
    public int? RejectCode { get; set; } = RejectCode;
    public string? RejectReason { get; set; } = RejectReason;
    public string? Cpm { get; set; } = Cpm;
    public string? TrType { get; set; } = TrType;
    public string? TerminalType { get; set; } = TerminalType;
    public long? VarVersion { get; set; } = VarVersion;
    public bool CanCancel { get; set; } = CanCancel;
    public bool CanChange { get; set; } = CanChange;
    public int PriceDigit { get; set; } = PriceDigit;
    public DateTime? OrderTime { get; set; } = OrderTime;
    public string? Logo { get; set; } = Logo;
    public string? InstrumentCategory { get; set; } = InstrumentCategory;
    public decimal? TickSize { get; set; } = TickSize;
    public decimal? LotSize { get; set; } = LotSize;
    public decimal? Multiplier { get; set; } = Multiplier;
    public MultiplierType? MultiplierType { get; set; } = MultiplierType;
    public MultiplierUnit? MultiplierUnit { get; set; } = MultiplierUnit;
}
