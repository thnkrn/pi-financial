namespace Pi.SetMarketData.Application.Queries.OrderBook;

public record GetOrderBookResponse(List<Domain.Entities.OrderBook> Data);