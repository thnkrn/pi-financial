using Pi.BackofficeService.Application.Factories;
using Pi.BackofficeService.Application.Queries.Filters;
using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;
using Pi.BackofficeService.Domain.AggregateModels.User;
using Pi.BackofficeService.Domain.Exceptions;

namespace Pi.BackofficeService.Application.Queries;

public class TicketQueries : ITicketQueries
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IResponseCodeRepository _responseCodeRepository;
    private readonly IUserRepository _userRepository;

    public TicketQueries(ITicketRepository ticketRepository, IResponseCodeRepository responseCodeRepository, IUserRepository userRepository)
    {
        _ticketRepository = ticketRepository;
        _responseCodeRepository = responseCodeRepository;
        _userRepository = userRepository;
    }

    public async Task<List<TicketResult>> GetTickets(int pageNum, int pageSize, string? orderBy, string? orderDirection, TicketFilters? filters)
    {
        var orderField = Enum.TryParse(orderBy, true, out TicketOrderable ticketOrderable) ? ticketOrderable : (TicketOrderable?)null;
        var records = await _ticketRepository.Get(pageNum, pageSize, orderField.ToString(), orderDirection, filters);

        return await _generateTicketResults(records);
    }

    public async Task<int> CountTickets(TicketFilters? filters)
    {
        return await _ticketRepository.Count(filters);
    }

    public async Task<List<TicketResult>> GetTicketsByTransactionNo(string transactionNo)
    {
        var records = await _ticketRepository.GetByTransactionNo(transactionNo);

        return await _generateTicketResults(records);
    }

    public async Task<TicketResult> GetTicketByTicketNo(string ticketNo)
    {
        var record = await _ticketRepository.GetByTicketNo(ticketNo);
        if (record == null)
        {
            throw new NotFoundException();
        }
        var results = await _generateTicketResults([record]);

        return results[0];
    }

    private async Task<List<TicketResult>> _generateTicketResults(List<TicketState> records)
    {
        var responseCodeIds = records.Where(q => q.ResponseCodeId != null)
            .Select(q => (Guid)q.ResponseCodeId!)
            .ToArray();
        var responseCodes = await _responseCodeRepository.Get(responseCodeIds);
        var userIds = Array.Empty<Guid>();
        records.ForEach(record =>
        {
            if (record.MakerId != null) userIds = userIds.Append((Guid)record.MakerId).ToArray();
            if (record.CheckerId != null) userIds = userIds.Append((Guid)record.CheckerId).ToArray();
        });
        var users = userIds.Length != 0 ? await _userRepository.GetIds(userIds) : new List<User>();

        return records.Select(q =>
        {
            var responseCode = responseCodes.Find(e => e.Id == q.ResponseCodeId);
            var maker = users.Find(e => e.Id == q.MakerId);
            var checker = users.Find(e => e.Id == q.CheckerId);

            return QueriesFactory.NewTicketResponse(q, maker, checker, responseCode);
        }).ToList();
    }
}
