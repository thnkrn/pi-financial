using Pi.StructureNotes.Infrastructure.Repositories.Entities;

namespace Pi.StructureNotes.Infrastructure.Repositories;

public static class Extensions
{
    public static Note ToDomainModel(this NoteEntity noteEntity) =>
        new()
        {
            Id = noteEntity.Id.ToString(),
            AccountId = noteEntity.AccountId,
            Currency = noteEntity.Currency,
            CostValue = noteEntity.CostValue,
            MarketValue = noteEntity.MarketValue,
            Isin = noteEntity.ISIN,
            Symbol = noteEntity.Symbol,
            Type = noteEntity.Type,
            Yield = noteEntity.Yield,
            Rebate = noteEntity.Rebate,
            Underlying = noteEntity.Underlying,
            Tenors = noteEntity.Tenors,
            TradeDate = noteEntity.TradeDate,
            IssueDate = noteEntity.IssueDate,
            AsOf = noteEntity.AsOf,
            ValuationDate = noteEntity.ValuationDate,
            Issuer = noteEntity.Issuer
        };

    public static Stock ToDomainModel(this StockEntity stockEntity) =>
        new()
        {
            Id = stockEntity.Id.ToString(),
            AccountId = stockEntity.AccountId,
            Symbol = stockEntity.Symbol,
            Currency = stockEntity.Currency,
            CostPrice = stockEntity.CostPrice,
            Available = stockEntity.Available,
            Units = stockEntity.Units,
            MarketValue = null,
            CostValue = null,
            MarketPrice = null,
            AsOf = stockEntity.AsOf
        };

    public static Cash ToDomainModel(this CashEntity cashEntity) =>
        new()
        {
            AccountId = cashEntity.AccountId,
            Id = cashEntity.Id.ToString(),
            Symbol = cashEntity.Symbol,
            Currency = cashEntity.Currency,
            CostValue = cashEntity.CostValue,
            CashGain = cashEntity.GainInPortfolio,
            MarketValue = cashEntity.MarketValue,
            AsOf = cashEntity.AsOf
        };
}
