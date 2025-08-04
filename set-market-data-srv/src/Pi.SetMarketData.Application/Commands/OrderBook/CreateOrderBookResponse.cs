namespace Pi.SetMarketData.Application.Commands.OrderBook;

public record CreateOrderBookResponse(bool Success, Domain.Entities.OrderBook? CreatedOrderBook = null);