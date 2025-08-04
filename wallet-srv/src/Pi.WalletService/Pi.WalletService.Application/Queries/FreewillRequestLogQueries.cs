using Pi.WalletService.Application.Factories;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Queries.Filters;
using Pi.WalletService.Application.Services;
using Pi.WalletService.Domain.AggregatesModel.LogAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using Transaction = Pi.WalletService.Domain.AggregatesModel.TransactionAggregate.Transaction;
using TransactionSummary = Pi.WalletService.Application.Models.TransactionSummary;

namespace Pi.WalletService.Application.Queries;

public class FreewillRequestLogQueries : IFreewillRequestLogQueries
{
    private readonly IFreewillRequestLogRepository _freewillRequestLogRepository;

    public FreewillRequestLogQueries(IFreewillRequestLogRepository freewillRequestLogRepository)
    {
        _freewillRequestLogRepository = freewillRequestLogRepository;
    }

    public async Task<List<FreewillRequestLog>> GetFreewillRequestLogs(FreewillRequestLogFilters filters)
    {
        var freewillRequestLogs = await _freewillRequestLogRepository.Get(
            filters.TransId,
            filters.Type,
            filters.CreatedAtFrom,
            filters.CreatedAtTo
        );

        return freewillRequestLogs;
    }
}