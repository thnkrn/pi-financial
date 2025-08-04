using Pi.SetService.Application.Models;

namespace Pi.SetService.Application.Services.PiInternalService;

public interface IPiInternalService
{
    Task<List<Trade>> GetTradeHistories(string accountNumber, DateOnly startDate, DateOnly endDate,
        CancellationToken ct = default
    );
    Task<BackofficeAvailableBalance?> GetBackofficeAvailableBalance(string accountNumber, CancellationToken ct = default);
}
