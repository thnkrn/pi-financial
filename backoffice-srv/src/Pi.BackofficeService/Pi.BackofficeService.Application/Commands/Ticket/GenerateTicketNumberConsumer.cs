using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;
using Pi.BackofficeService.Domain.Events.Ticket;

namespace Pi.BackofficeService.Application.Commands.Ticket;

public record GenerateTicketNoMessage(Guid CorrelationId);

public class GenerateTicketNoConsumer : IConsumer<GenerateTicketNoMessage>
{
    private readonly ILogger<GenerateTicketNoConsumer> _logger;
    private readonly ITicketRepository _ticketRepository;
    private const string Prefix = "TIC";
    private const int MaxRecursion = 5;

    public GenerateTicketNoConsumer(ITicketRepository ticketRepository, ILogger<GenerateTicketNoConsumer> logger)
    {
        _ticketRepository = ticketRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<GenerateTicketNoMessage> context)
    {
        var ticketNo = await GenerateTicketNo(context.Message.CorrelationId, 0);

        await context.RespondAsync(new TicketNoGeneratedResponse(context.Message.CorrelationId, ticketNo));
    }

    private async Task<string> GenerateTicketNo(Guid correlationId, int depth)
    {
        var latestTicketNo = await _ticketRepository.GetLatestTicketNo();
        var latest = latestTicketNo == null ? 0 : int.Parse(latestTicketNo.Replace(Prefix, ""));
        var ticketNo = Prefix + (latest + 1).ToString("D4");

        try
        {
            _logger.LogInformation("Ticket Number: {TicketNo}", ticketNo);
            await _ticketRepository.UpdateTicketNo(correlationId, ticketNo);
        }
        catch (DuplicateTicketNoException ex)
        {
            if (depth < MaxRecursion) return await GenerateTicketNo(correlationId, depth + 1);

            _logger.LogCritical(ex, "Unable to Generate Ticket Number. Message: {Message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Unable to Generate Ticket Number. Message: {Message}", ex.Message);
            throw;
        }

        return ticketNo;
    }
}
