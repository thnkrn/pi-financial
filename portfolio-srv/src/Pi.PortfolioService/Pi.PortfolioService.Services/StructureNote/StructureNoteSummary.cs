namespace Pi.PortfolioService.Application.Services.Models.StructureNote;

public record StructureNoteAccountSummary(
    string AccountType,
    string AccountId,
    string AccountNoForDisplay,
    string CustCode,
    bool SblFlag,
    decimal TotalMarketValue,
    decimal Upnl,
    string ErrorMessage
)
{
    public decimal TotalValue => TotalMarketValue;
    public decimal TotalCostValue => TotalValue - Upnl;
};
