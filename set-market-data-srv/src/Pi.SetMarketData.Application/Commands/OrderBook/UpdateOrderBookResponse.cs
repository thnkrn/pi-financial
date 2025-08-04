namespace Pi.SetMarketData.Application.Commands.OrderBook;

public record UpdateOrderBookResponse(bool Success, Domain.Entities.OrderBook? UpdatedOrderBook = null);