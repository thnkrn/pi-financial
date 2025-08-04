using Pi.WalletService.Application.Models;
using Pi.WalletService.Domain.AggregatesModel.LogAggregate;

namespace Pi.WalletService.Application.Queries;

public record FreewillRequestLogFilters(
    string? TransId,
    FreewillRequestType? Type,
    DateTime? CreatedAtFrom,
    DateTime? CreatedAtTo
);

public interface IFreewillRequestLogQueries
{
    Task<List<FreewillRequestLog>> GetFreewillRequestLogs(FreewillRequestLogFilters filters);
}